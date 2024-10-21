using Azure.Messaging.ServiceBus;
using Elastic.Apm;
using Microsoft.AspNetCore.Mvc;

namespace DemoApm.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "GetWeatherTemperature")]
    public async Task<int> Get()
    {
        // Customizations
        // https://www.elastic.co/guide/en/apm/agent/nodejs/current/transaction-api.html#transaction-start-span

        var currentTransaction = Agent.Tracer.CurrentTransaction;

        currentTransaction.Name = "CustomRoute";
        currentTransaction.SetLabel("CustomLabel1", "testvalue1");
        currentTransaction.SetLabel("CustomLabel2", "testvalue2");

        currentTransaction.Custom.Add("MyCustomField", "testcustomvalue");

        var client = new ServiceBusClient("you-ServiceBus-connection-string");
        
        var sender = client.CreateSender("apmtest");

        await sender.SendMessageAsync(new ServiceBusMessage($"Temperature is: 5"));

        return 5;
    }

    [HttpGet("exception", Name = "GetWeatherTemperatureException")]
    public void GetWeatherTemperatureException()
    {
        throw new ApplicationException("Test exception.");
    }
}