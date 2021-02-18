using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POCAzureServiceBus
{
    static class Send
    {
        private static Queue<ServiceBusMessage> CreateMessages()
        {
            // create a queue containing the messages and return it to the caller
            Queue<ServiceBusMessage> messages = new Queue<ServiceBusMessage>();
            for (int i = 0; i < 1000; i++)
            {
                messages.Enqueue(new ServiceBusMessage($"m-{i}"));
            }
            return messages;
        }

        public static async Task SendMessageBatchAsync()
        {
            // create a Service Bus client 
            await using (ServiceBusClient client = new ServiceBusClient(Config.ConnectionString))
            {
                // create a sender for the queue 
                ServiceBusSender sender = client.CreateSender(Config.QueueName);

                // get the messages to be sent to the Service Bus queue
                Queue<ServiceBusMessage> messages = CreateMessages();

                // total number of messages to be sent to the Service Bus queue
                int messageCount = messages.Count;

                // while all messages are not sent to the Service Bus queue
                while (messages.Count > 0)
                {
                    // start a new batch 
                    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                    // add the first message to the batch
                    if (messageBatch.TryAddMessage(messages.Peek()))
                    {
                        // dequeue the message from the .NET queue once the message is added to the batch
                        messages.Dequeue();
                    }
                    else
                    {
                        // if the first message can't fit, then it is too large for the batch
                        throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                    }

                    // add as many messages as possible to the current batch
                    while (messages.Count > 0 && messageBatch.TryAddMessage(messages.Peek()))
                    {
                        // dequeue the message from the .NET queue as it has been added to the batch
                        messages.Dequeue();
                    }

                    // now, send the batch
                    await sender.SendMessagesAsync(messageBatch);
                }
                Console.WriteLine($"Sent a batch of {messageCount} messages to the topic: {Config.QueueName}");
            }
        }

        public static async Task SendMessageAsync()
        {
            // create a Service Bus client 
            await using (ServiceBusClient client = new ServiceBusClient(Config.ConnectionString))
            {
                // create a sender for the queue 
                ServiceBusSender sender = client.CreateSender(Config.QueueName);

                // create a message that we can send
                ServiceBusMessage message = new ServiceBusMessage("Mensam");

                // send the message
                await sender.SendMessageAsync(message);
                Console.WriteLine($"Sent a single message to the queue: {Config.QueueName}");
            }
        }
    }
}
