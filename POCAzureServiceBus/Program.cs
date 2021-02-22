using System.Threading.Tasks;

namespace POCAzureServiceBus
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // send a batch of messages to the queue
            // await Send.SendMessageBatchAsync();

            // send a message to the queue
            //await Send.SendMessageAsync();

            await Receive.ReceiveMessagesAsync();
        }
    }
}
