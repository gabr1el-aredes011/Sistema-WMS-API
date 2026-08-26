using Wms.Infrastructure;
using Wms.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

const string frontendCorsPolicy = "Frontend";
var frontendOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        if (frontendOrigins.Length > 0)
        {
            policy
                .WithOrigins(frontendOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var connectionString =
    builder.Configuration.GetConnectionString("WmsDatabase")
    ?? throw new InvalidOperationException(
        "A connection string 'WmsDatabase' não foi configurada.");

builder.Services.AddInfrastructure(
    connectionString,
    builder.Configuration);
var app = builder.Build();

await app.Services.SeedDevelopmentAdminAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(frontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/api/v1/health/ready");

app.Run();

public partial class Program;
