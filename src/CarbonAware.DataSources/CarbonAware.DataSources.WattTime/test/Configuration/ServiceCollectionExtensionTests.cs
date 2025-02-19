using CarbonAware.Configuration;
using CarbonAware.DataSources.WattTime.Client;
using CarbonAware.DataSources.WattTime.Configuration;
using CarbonAware.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;

namespace CarbonAware.DataSources.WattTime.Tests;

[TestFixture]
class ServiceCollectionExtensionTests
{
    private readonly string ForecastDataSourceKey = $"DataSources:ForecastDataSource";
    private readonly string ForecastDataSourceValue = $"WattTimeTest";
    private readonly string UsernameKey = $"DataSources:Configurations:WattTimeTest:Username";
    private readonly string Username = "devuser";
    private readonly string PasswordKey = $"DataSources:Configurations:WattTimeTest:Password";
    private readonly string Password = "12345";
    private readonly string ProxyUrl = $"DataSources:Configurations:WattTimeTest:Proxy:Url";
    private readonly string UseProxyKey = $"DataSources:Configurations:WattTimeTest:Proxy:UseProxy";

    [Test]
    public void ClientProxyTest_With_Invalid_ProxyURL_ThrowsException()
    {
        // Arrange
        var settings = new Dictionary<string, string> {
            { ForecastDataSourceKey, ForecastDataSourceValue },
            { UsernameKey, Username },
            { PasswordKey, Password },
            { ProxyUrl, "http://fakeproxy:8080" },
            { UseProxyKey, "true" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .AddEnvironmentVariables()
            .Build();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWattTimeForecastDataSource(configuration.DataSources());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IWattTimeClient>();

        // Act & Assert
        Assert.ThrowsAsync<HttpRequestException>(async () => await client.GetBalancingAuthorityAsync("lat", "long"));
    }

    [Test]
    public void ClientProxyTest_With_Missing_ProxyURL_ThrowsException()
    {
        // Arrange
        var settings = new Dictionary<string, string> {
            { ForecastDataSourceKey, ForecastDataSourceValue },
            { UsernameKey, Username },
            { PasswordKey, Password },
            { UseProxyKey, "true" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .AddEnvironmentVariables()
            .Build();
        var serviceCollection = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ConfigurationException>(() => serviceCollection.AddWattTimeForecastDataSource(configuration.DataSources()));
    }
}