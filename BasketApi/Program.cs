using BaseApi.Extensions;
using BasketApi.Domain.Interfaces;
using BasketApi.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;
using Sentry;

var builder = WebApplication.CreateBuilder(args);

// Sentry (error tracking + tracing)
builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["Sentry:Dsn"] ?? "";
    o.TracesSampleRate = double.TryParse(builder.Configuration["Sentry:TracesSampleRate"], out var tsr) ? tsr : 0.2;
    o.ProfilesSampleRate = double.TryParse(builder.Configuration["Sentry:ProfilesSampleRate"], out var psr) ? psr : 0.1;
    o.SendDefaultPii = false;
});

// Add BaseInfrastructure (Redis, Health, etc. from BaseApi submodule)
builder.Services.AddBaseInfrastructure(builder.Configuration);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddConsulConfig(builder.Configuration);
}

// Register DDD Repository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseBaseInfrastructure("Basket API", "Basket API - Inventory & Shopping");

if (!app.Environment.IsEnvironment("Testing"))
{
    app.RegisterWithConsul(builder.Configuration, app.Lifetime);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.Run();
