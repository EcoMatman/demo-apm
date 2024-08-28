using Azure.Messaging.ServiceBus;
using Elastic.Apm;
using Elastic.Apm.Api;

namespace DemoApmReceiver;

public class DemoApmServiceBusReceiver
{
    public static async Task Receive()
    {
        var client = new ServiceBusClient(
            "your-ServiceBus-connection-string");

        var processor = client.CreateProcessor("apmtest", new ServiceBusProcessorOptions());

        try
        {
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            
            Console.ReadKey();
            await processor.StopProcessingAsync();
            Console.ReadKey();
        }
        finally
        {
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }
    }

    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        await args.CompleteMessageAsync(args.Message);
    }
}