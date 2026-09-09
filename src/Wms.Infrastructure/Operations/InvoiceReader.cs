using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace Wms.Infrastructure.Operations;

public static class InvoiceReader
{
    public sealed record InvoiceItem(string Description, decimal Quantity, string Unit);
    public static (string Key, string Number, List<InvoiceItem> Items) Read(byte[] xml)
    {
        using var stream = new MemoryStream(xml);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 2_000_000
        });
        var doc = XDocument.Load(reader);
        XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        var infos = doc.Descendants(ns + "infNFe").ToList();
        if (infos.Count != 1) throw new ArgumentException("Selecione exatamente uma NF-e.");
        var info = infos[0];
        var key = ((string?)info.Attribute("Id"))?.Replace("NFe", "") ?? "";
        var number = info.Element(ns + "ide")?.Element(ns + "nNF")?.Value ?? "";
        if (key.Length != 44 || !key.All(char.IsAsciiDigit) || number.Length is < 1 or > 9 || !number.All(char.IsAsciiDigit))
            throw new ArgumentException("A chave ou o número da NF-e é inválido.");
        var items = info.Elements(ns + "det").Select(x =>
        {
            var p = x.Element(ns + "prod");
            if (!decimal.TryParse(p?.Element(ns + "qCom")?.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var quantity) || quantity <= 0 || decimal.Round(quantity, 3) != quantity)
                throw new ArgumentException("Quantidade da NF-e inválida ou com mais de três casas decimais.");
            return new InvoiceItem(p?.Element(ns + "xProd")?.Value ?? "", quantity, p?.Element(ns + "uCom")?.Value ?? "");
        }).ToList();
        if (items.Count is < 1 or > 200) throw new ArgumentException("A NF-e deve conter entre 1 e 200 itens.");
        return (key, number, items);
    }

    public static byte[] Decode(string text, int limit, string kind)
    {
        if (string.IsNullOrEmpty(text) || text.Length > (limit + 2L) / 3 * 4)
            throw new ArgumentException($"Arquivo {kind} ausente ou acima do limite.");
        byte[] bytes;
        try { bytes = Convert.FromBase64String(text); }
        catch (FormatException) { throw new ArgumentException($"Arquivo {kind} inválido."); }
        if (bytes.Length == 0 || bytes.Length > limit) throw new ArgumentException($"Arquivo {kind} acima do limite.");
        if (kind == "PDF" && (bytes.Length < 5 || Encoding.ASCII.GetString(bytes, 0, 5) != "%PDF-"))
            throw new ArgumentException("O anexo informado não é um PDF.");
        return bytes;
    }
}
