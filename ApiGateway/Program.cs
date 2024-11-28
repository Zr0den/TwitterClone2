using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Polly.Extensions.Http;
using Polly;
using Ocelot.Values;
using Microsoft.IdentityModel.Tokens;

using System.Text;

// Security key for decoding AuthToken
// 
const string SecurityKey = "mMdAhocQbIAa1/4iD8W5BiDCD9Lxg9ULp4qROgJVN8oRZommyAsnRalnNlzWGbKGJItr/kh2jVd2d9brhSBAJttV7NE47dvyX6n36cFKlnz3k9AodqqVgH/S52oQMYamtI+HsQqBmsvZMqOE+oGlEIzJG9tmDZ1JE/qJHq+bXo3RCEuBf26dGuIG4DWpjh+G4xTVC7ZoByCmq5zTUUyTlFZCQ2483iJe1Thkem9mlzt3cOy8O5SYJBafIb0xdIBYEoHl56Z805fO/W4eAw+M5stSCUdJTBUtWbCiId9zSapmilb20sCg4l5xYTsaJImTfHlo0t9kF1o/RXwr1cw3zCPoyt9tjWhZ83LMsi1ydBg=";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", false, false);

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

// Register HttpClient with Polly policies
builder.Services.AddHttpClient("ExternalServiceClient")
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError() // Retry for transient errors
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddPolicyHandler(circuitBreakerPolicy);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey))
        };
    }
);
builder.Services.AddOcelot(builder.Configuration);
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

app.UseOcelot().Wait();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
