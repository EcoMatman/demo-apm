using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace DemoApm.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherTemperature")]
    public async Task<int> Get()
    {
        var client = new ServiceBusClient("you-ServiceBus-connection-string");
        var sender = client.CreateSender("demoapmtopic");
        
        var wf = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        
        var sender = client.CreateSender("apmtest");

        await sender.SendMessageAsync(new ServiceBusMessage($"Temperature is: 5"));

        return 5;
    }
}