using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace POCAzureServiceBus
{
    static class Receive
    {
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            //process
            //System.Threading.Thread.Sleep(60000);
            
            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);

        }

        // handle any errors when receiving messages
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        public static async Task ReceiveMessagesAsync()
        {
            await using (ServiceBusClient client = new ServiceBusClient(Config.ConnectionString))
            {
                // create a processor that we can use to process the messages
                ServiceBusProcessor processor = client.CreateProcessor(Config.QueueName, new ServiceBusProcessorOptions());

                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
        }
    }
}
