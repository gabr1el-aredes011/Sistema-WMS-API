using System.Text;
using Microsoft.EntityFrameworkCore;
using Wms.Application.Operations;
using Wms.Domain.Catalog;
using Wms.Domain.Operations;
using Wms.Infrastructure.Operations;
using Wms.Infrastructure.Persistence;

namespace Wms.IntegrationTests;

public sealed class OperationsDatabaseFactAttribute : FactAttribute
{
    public OperationsDatabaseFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WMS_OPERATIONS_TEST_DB")))
            Skip = "Requires an isolated PostgreSQL database in WMS_OPERATIONS_TEST_DB.";
    }
}

public sealed class OperationsTests
{
    private static string Xml(int number, decimal quantity = 10) => $"""
        <nfeProc xmlns="http://www.portalfiscal.inf.br/nfe"><NFe><infNFe Id="NFe{number.ToString().PadLeft(44, '0')}">
        <ide><nNF>{number}</nNF></ide><det nItem="1"><prod><cProd>TEST</cProd><xProd>Peça de teste</xProd><qCom>{quantity}</qCom><uCom>UN</uCom></prod></det>
        </infNFe></NFe></nfeProc>
        """;
    [Fact]
    public void InvoiceReaderRejectsEntities() => Assert.Throws<System.Xml.XmlException>(() => InvoiceReader.Read(Encoding.UTF8.GetBytes("<!DOCTYPE a [<!ENTITY ext SYSTEM 'file:///secret'>]><a>&ext;</a>")));
    [Fact]
    public void InvoiceReaderExtractsFullIdentityAndItems()
    {
        var invoice = InvoiceReader.Read(Encoding.UTF8.GetBytes(Xml(1234)));
        Assert.Equal("1234", invoice.Number); Assert.Equal(44, invoice.Key.Length); Assert.Equal(10, Assert.Single(invoice.Items).Quantity);
    }
    [Fact]
    public void PdfMustHaveSignature() => Assert.Throws<ArgumentException>(() => InvoiceReader.Decode(Convert.ToBase64String(Encoding.UTF8.GetBytes("not a pdf")), 100, "PDF"));

    [OperationsDatabaseFact]
    public async Task ReservationsProductionCancellationAndDispatchRemainConsistentUnderConcurrency()
    {
        var connection = Environment.GetEnvironmentVariable("WMS_OPERATIONS_TEST_DB")!;
        WmsDbContext Context() => new(new DbContextOptionsBuilder<WmsDbContext>().UseNpgsql(connection).Options);
        await using var setup = Context();
        await setup.Database.MigrateAsync();
        var categoryName = "TESTE " + Guid.NewGuid();
        var category = new ProductCategory { Name = categoryName, NormalizedName = categoryName, CreatedAtUtc = DateTimeOffset.UtcNow };
        var product = new Product { Category = category, Name = "Teste", NormalizedName = "TESTE", Type = "Componente", CreatedAtUtc = DateTimeOffset.UtcNow };
        var variant = new ProductVariant { Product = product, Color = "Azul", CreatedAtUtc = DateTimeOffset.UtcNow };
        setup.Add(variant); await setup.SaveChangesAsync();
        async Task Run(Func<OperationsService, Task> action) { await using var db = Context(); await action(new OperationsService(db)); }
        var receipt = new ProductionInput(Guid.NewGuid(), variant.Id, 6);
        await Run(s => s.Produce(receipt, "Teste", default));
        await Run(s => s.Produce(receipt, "Teste", default));
        CreateOrderInput Order(int n) => new(Guid.NewGuid(), "WhatsApp", "Cliente teste", null,
            [new(variant.Id, "Peça de teste", 10, false, null)], Convert.ToBase64String(Encoding.ASCII.GetBytes("%PDF-test")), Convert.ToBase64String(Encoding.UTF8.GetBytes(Xml(n))));
        var invoiceNumber = Random.Shared.Next(1000000, 900000000);
        var one = Order(invoiceNumber); var two = Order(invoiceNumber + 1);
        await Run(s => s.Create(one, "Escritório", default));
        await Run(s => s.Create(two, "Escritório", default));
        await using (var read = Context())
        {
            Assert.Equal(6, await read.Set<OrderLine>().Where(x => x.SalesOrderId == one.Id).SumAsync(x => x.Reserved));
            Assert.Equal(0, await read.Set<OrderLine>().Where(x => x.SalesOrderId == two.Id).SumAsync(x => x.Reserved));
        }
        await Run(s => s.Act(one.Id, new("Separating", null, null, null), "Operador", default));
        await Assert.ThrowsAsync<ArgumentException>(() => Run(s => s.Act(one.Id, new("Ready", null, null, null, true), "Operador", default)));
        var concurrent = new ProductionInput(Guid.NewGuid(), variant.Id, 8);
        await Task.WhenAll(Run(s => s.Produce(concurrent, "Fábrica", default)), Run(s => s.Produce(concurrent, "Fábrica", default)));
        await using (var read = Context())
        {
            var balance = await read.Set<StockBalance>().SingleAsync(x => x.VariantId == variant.Id); Assert.Equal(14, balance.OnHand); Assert.Equal(14, balance.Reserved);
            Assert.Equal(10, await read.Set<OrderLine>().Where(x => x.SalesOrderId == one.Id).SumAsync(x => x.Reserved));
        }
        await Run(s => s.Act(one.Id, new("Cancelled", null, null, null), "Escritório", default));
        await Run(s => s.Act(two.Id, new("Separating", null, null, null), "Operador", default));
        await Run(s => s.Act(two.Id, new("Assign", null, "Separador", null), "Operador", default));
        await Run(s => s.Act(two.Id, new("Ready", null, null, null, true), "Operador", default));
        await Run(s => s.Act(two.Id, new("Dispatched", null, null, null), "Operador", default));
        await Assert.ThrowsAsync<ArgumentException>(() => Run(s => s.Act(two.Id, new("Dispatched", null, null, null), "Operador", default)));
        await using (var read = Context())
        {
            var balance = await read.Set<StockBalance>().SingleAsync(x => x.VariantId == variant.Id); Assert.Equal(4, balance.OnHand); Assert.Equal(0, balance.Reserved);
            Assert.Equal(4, await read.Set<StockMovement>().Where(x => x.VariantId == variant.Id).SumAsync(x => x.Quantity));
            Assert.Equal(4, await read.Set<OrderDocument>().Where(x => x.SalesOrderId == one.Id || x.SalesOrderId == two.Id).CountAsync());
        }
        var duplicate = one with { Id = Guid.NewGuid() };
        await Assert.ThrowsAsync<ArgumentException>(() => Run(s => s.Create(duplicate, "Teste", default)));
        var custom = Order(invoiceNumber + 2) with { Lines = [new(null, "Peça de teste", 10, true, "Acabamento especial")], Attachments = [new("referencia.pdf", Convert.ToBase64String("%PDF-test"u8.ToArray()))] };
        await Run(s => s.Create(custom, "Escritório", default));
        await Run(s => s.Act(custom.Id, new("Separating", null, null, null), "Operador", default));
        await Run(s => s.Act(custom.Id, new("Assign", null, "Separador", null), "Operador", default));
        await Assert.ThrowsAsync<ArgumentException>(() => Run(s => s.Act(custom.Id, new("Ready", null, null, null, true), "Operador", default)));
        Guid customLineId;
        await using (var read = Context()) { customLineId = await read.Set<OrderLine>().Where(x => x.SalesOrderId == custom.Id).Select(x => x.Id).SingleAsync(); }
        await Run(s => s.Act(custom.Id, new("CustomComplete", null, null, customLineId), "Fábrica", default));
        await Run(s => s.Act(custom.Id, new("Ready", null, null, null, true), "Operador", default));
        await Run(async s => Assert.Equal(3, (await s.Documents(custom.Id, default)).Count));
        var third = Order(invoiceNumber + 3); var fourth = Order(invoiceNumber + 4);
        await Task.WhenAll(Run(s => s.Create(third, "Escritório", default)), Run(s => s.Create(fourth, "Escritório", default)));
        await using (var read = Context())
        {
            var b = await read.Set<StockBalance>().SingleAsync(x => x.VariantId == variant.Id);
            Assert.Equal(4, b.Reserved); Assert.Equal(0, b.OnHand - b.Reserved);
        }
        await using (var edit = Context()) { var v = await edit.ProductVariants.SingleAsync(x => x.Id == variant.Id); v.IsActive = false; await edit.SaveChangesAsync(); }
        var inactive = await Assert.ThrowsAsync<ArgumentException>(() => Run(s => s.Produce(new(Guid.NewGuid(), variant.Id, 1), "Teste", default)));
        Assert.Equal("Produto Inativado do Sistema", inactive.Message);
    }
}
