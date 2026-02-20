using BaseApi.Extensions;
using BasketApi.Domain.Interfaces;
using BasketApi.Infrastructure.Persistence;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

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
