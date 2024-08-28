using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoApmReceiver;

public static class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configBuilder => { configBuilder.AddEnvironmentVariables(); })
            .ConfigureAppConfiguration(configBuilder => { configBuilder.AddJsonFile("appsettings.json"); })
            .ConfigureServices((host, services) =>
            {
                host.HostingEnvironment.EnvironmentName = "Development";
                services.AddAllElasticApm();
            });

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        using var host = CreateHostBuilder(args).Build();
        await host.StartAsync();
        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        
        await DemoApmServiceBusReceiver.Receive();

        lifetime.StopApplication();
        await host.WaitForShutdownAsync();
    }
}