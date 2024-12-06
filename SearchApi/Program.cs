using Helpers.RabbitMQ;
using Polly;
using Polly.Extensions.Http;
using Vault;
using Vault.Model;
using Vault.Client;
using SearchApi;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.GetSection("Settings").Get<Settings>();

var circuitBreakerPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(response => (int)response.StatusCode >= 500)
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, timespan) =>
        {
            Console.WriteLine($"Circuit breaker opened due to: {outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase}");
            Console.WriteLine($"Circuit will remain open for {timespan.TotalSeconds} seconds.");
        },
        onReset: () => Console.WriteLine("Circuit breaker reset. Normal operation resumed."),
        onHalfOpen: () => Console.WriteLine("Circuit breaker is half-open. Testing next request.")
    );

// Registering HttpClient with Polly policies
builder.Services.AddHttpClient("ExternalServiceClient")
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError() // Retry for transient errors
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddPolicyHandler(circuitBreakerPolicy);
// Add services to the container.
builder.Services.AddSingleton(
    (ServiceProvider) =>
    {
        VaultConfiguration vaultConfiguration = new VaultConfiguration(config.VaultHostname);
        VaultClient vaultClient = new VaultClient(vaultConfiguration);

        var authResponse = vaultClient.Auth.UserpassLogin("Vaultuser", new UserpassLoginRequest("Vaultpass"));
        vaultClient.SetToken(authResponse.ResponseAuth.ClientToken);

        VaultResponse<KvV2ReadResponse> response = vaultClient.Secrets.KvV2Read("secret", "kv");
        JObject data = (JObject)response.Data.Data;

        Console.WriteLine(data.ToString());

        SecretSettings secretSettings = data.ToObject<SecretSettings>();
        return secretSettings;
    }
);
// Add RabbitMQ settings from appsettings.json
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
// Register RabbitMQProducer and RabbitMQConsumer
builder.Services.AddSingleton<RabbitMQProducer>();
builder.Services.AddSingleton<RabbitMQConsumer>(); 
builder.Services.AddHostedService<RabbitMQConsumerHostedService>(); // Background service to run RabbitMQConsumer

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
