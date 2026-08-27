using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wms.Api.Controllers;
using Wms.Application.Authorization;
using Wms.Infrastructure;

namespace Wms.IntegrationTests;

public sealed class PermissionPolicyTests
{
    [Fact]
    public async Task EverySystemPermission_HasAClaimPolicy()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Wms.Tests",
                ["Jwt:Audience"] = "Wms.Tests.Client",
                ["Jwt:SigningKey"] = "integration-tests-signing-key-with-32-bytes"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(
            "Host=localhost;Database=wms_tests;Username=test;Password=test",
            configuration);
        await using var provider = services.BuildServiceProvider();
        var policyProvider = provider
            .GetRequiredService<IAuthorizationPolicyProvider>();

        foreach (var permission in SystemPermissions.All)
        {
            var policy = await policyProvider.GetPolicyAsync(permission.Code);
            var claimRequirement = Assert.Single(
                policy!.Requirements.OfType<ClaimsAuthorizationRequirement>());

            Assert.Equal("permission", claimRequirement.ClaimType);
            Assert.Contains(
                permission.Code,
                claimRequirement.AllowedValues!);
        }
    }

    [Fact]
    public void CreateUser_RequiresCreationAndRoleManagementPermissions()
    {
        var method = typeof(UsersController).GetMethod(
            nameof(UsersController.Create));
        var policies = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy)
            .ToArray();

        Assert.Contains(SystemPermissions.Users.Create, policies);
        Assert.Contains(SystemPermissions.Users.ManageRoles, policies);
    }

    [Theory]
    [InlineData(nameof(ProductsController.GetAll), SystemPermissions.Products.Read)]
    [InlineData(nameof(ProductsController.GetById), SystemPermissions.Products.Read)]
    [InlineData(nameof(ProductsController.Create), SystemPermissions.Products.Create)]
    [InlineData(nameof(ProductsController.Update), SystemPermissions.Products.Update)]
    [InlineData(nameof(ProductsController.SetStatus), SystemPermissions.Products.Disable)]
    public void ProductEndpoint_RequiresExpectedPermission(
        string methodName,
        string expectedPolicy)
    {
        var method = typeof(ProductsController).GetMethod(methodName);
        var policies = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .Select(attribute => attribute.Policy);

        Assert.Contains(expectedPolicy, policies);
    }
}
