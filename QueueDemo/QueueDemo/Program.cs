using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace QueueDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=logicstorageacc2021;AccountKey=ja26nqkxf+tthmVqFsGJM3Ij7aP2XT7Eao2c6jBh8eJOB+BVtGsQk5siIMml+K7aHlZ0G5y31qM2kWNg8oRCig==;EndpointSuffix=core.windows.net";

            QueueClient queue = new QueueClient(connectionString, "mystoragequeue");

            await InsertMessageAsync(queue, "message1");
            await InsertMessageAsync(queue, "message2");
            await InsertMessageAsync(queue, "message3");
            await InsertMessageAsync(queue, "message4");

            //if (args.Length > 0)
            //{
            //    string value = String.Join(" ", args);
            //    await InsertMessageAsync(queue, value);
            //    Console.WriteLine($"Sent: {value}");
            //}
            //else
            //{
            //    string value = await RetrieveNextMessageAsync(queue);
            //    Console.WriteLine($"Received: {value}");
            //}

            Console.Write("Press Enter...");
            Console.ReadLine();
        }

        static async Task InsertMessageAsync(QueueClient theQueue, string newMessage)
        {
            if (null != await theQueue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue was created.");
            }

            await theQueue.SendMessageAsync(newMessage);
        }

        static async Task<string> RetrieveNextMessageAsync(QueueClient theQueue)
        {
            if (await theQueue.ExistsAsync())
            {
                QueueProperties properties = await theQueue.GetPropertiesAsync();

                if (properties.ApproximateMessagesCount > 0)
                {
                    QueueMessage[] retrievedMessage = await theQueue.ReceiveMessagesAsync(1);
                    string theMessage = retrievedMessage[0].Body.ToString();
                    await theQueue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                    return theMessage;
                }
                else
                {
                    Console.Write("The queue is empty. Attempt to delete it? (Y/N) ");
                    string response = Console.ReadLine();

                    if (response.ToUpper() == "Y")
                    {
                        await theQueue.DeleteIfExistsAsync();
                        return "The queue was deleted.";
                    }
                    else
                    {
                        return "The queue was not deleted.";
                    }
                }
            }
            else
            {
                return "The queue does not exist. Add a message to the command line to create the queue and store the message.";
            }
        }
    }
}
