using Azure.Messaging.ServiceBus;
using Elastic.Apm;
using Elastic.Apm.Api;

namespace DemoApmReceiver;

public class DemoApmServiceBusReceiver
{
    public static async Task Receive(HttpClient httpClient)
    {
        var client = new ServiceBusClient(
            "your-ServiceBus-connection-string");

        var processor = client.CreateProcessor("apmtest", new ServiceBusProcessorOptions());

        var counter = 0;

        try
        {
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.ReadKey();
            await processor.StopProcessingAsync();
        }
        finally
        {
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }

        return;

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            counter++;
            if (counter >= 2)
            {
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            await httpClient.GetStringAsync("https://localhost:44385/WeatherForecast");

            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}