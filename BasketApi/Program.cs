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

// Register DDD Repository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Add FastEndpoints
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Basket API";
        s.Version = "v1";
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseBaseInfrastructure();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseFastEndpoints();

app.Run();
