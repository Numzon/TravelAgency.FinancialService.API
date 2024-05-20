using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using System.Reflection;
using TravelAgency.FinancialService.API.Common.Settings;
using TravelAgency.FinancialService.API.Configurations;
using TravelAgency.FinancialService.API.Infrastructure.Persistance;
using TravelAgency.FinancialService.API.Mapster;
using TravelAgency.SharedLibrary.AWS;
using TravelAgency.SharedLibrary.Models;
using TravelAgency.SharedLibrary.RabbitMQ;
using TravelAgency.SharedLibrary.Vault.Consts;
using TravelAgency.SharedLibrary.Vault;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints()
    .SwaggerDocument(options =>
{
    options.AutoTagPathSegmentIndex = 0;
});

builder.Services.RegisterMapsterConfiguration();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

if (builder.Environment.IsProduction())
{
    var vaultBuilder = new VaultFacadeBuilder();

    var vaultFacade = vaultBuilder
                        .SetToken(Environment.GetEnvironmentVariable(VaultEnvironmentVariables.Token))
                        .SetPort(Environment.GetEnvironmentVariable(VaultEnvironmentVariables.Port))
                        .SetHost(Environment.GetEnvironmentVariable(VaultEnvironmentVariables.Host))
                        .SetSSL(false)
                        .Build();

    var rabbitMq = await vaultFacade.ReadRabbitMqSecretAsync();
    var database = await vaultFacade.ReadFinancialServiceDatabaseSecretAsync();
    var cognito = await vaultFacade.ReadCognitoSecretAsync();

    builder.Configuration.AddInMemoryCollection(rabbitMq);
    builder.Configuration.AddInMemoryCollection(database);
    builder.Configuration.AddInMemoryCollection(cognito);
}

builder.Services.RegisterDatabase();
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

builder.Services.AddSingleton<FinancialServiceDbContext>();
builder.Services.AddScoped<FinancialServiceDbContextInitialiser>();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetRequiredSection("Database"));
builder.Services.Configure<RabbitMqSettingsDto>(builder.Configuration.GetRequiredSection("RabbitMQ"));
builder.Services.Configure<AwsCognitoSettingsDto>(builder.Configuration.GetRequiredSection("AWS:Cognito"));

try
{
    var cognitoConfiguration = builder.Configuration.GetRequiredSection("AWS:Cognito").Get<AwsCognitoSettingsDto>()!;
    builder.Services.AddAuthenticationAndJwtConfiguration(cognitoConfiguration);
}
catch (Exception ex)
{
    Log.Error(ex.Message);
}

try
{
    var rabbitMqSettings = builder.Configuration.GetRequiredSection("RabbitMQ").Get<RabbitMqSettingsDto>()!;
    builder.Services.AddRabbitMqConfiguration(rabbitMqSettings);
}
catch (Exception ex)
{
    Log.Error(ex.Message);
}

builder.Services.AddSingleton(EventStrategyConfiguration.GetGlobalSettingsConfiguration());

builder.Services.AddAuthorizationWithPolicies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<FinancialServiceDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseFastEndpoints()
    .UseDefaultExceptionHandler()
    .UseSwaggerGen();

app.Run();

#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors