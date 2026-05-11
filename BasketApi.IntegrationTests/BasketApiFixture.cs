extern alias BasketApiApp;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BasketApi.IntegrationTests;

public class BasketApiFixture : AppFixture<BasketApiApp::Program>
{
    public Mock<BasketApiApp::BasketApi.Domain.Interfaces.IBasketRepository> BasketRepositoryMock { get; } = new();

    protected override ValueTask SetupAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(BasketApiApp::BasketApi.Domain.Interfaces.IBasketRepository));
            if (descriptor != null) services.Remove(descriptor);

            services.AddSingleton(BasketRepositoryMock.Object);
        });
    }
}
