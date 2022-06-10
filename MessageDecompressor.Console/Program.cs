using System;
using MessageDecompressorWpf.Library;

namespace MessageDecompressorWpf
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: message_decompressor <connection-string> <queue-name>");
                return;
            }

            await GetMessages(args[0], args[1]);
        }

        private async static Task GetMessages(string connectionString, string queueName)
        {
            try
            {
                var messages = await MessageDecompressorLib.GetDecompressedMessages(connectionString, queueName);

                foreach (var message in messages)
                {
                    Console.WriteLine($"Message ID = {message.messageId}\n\n{message.messageJson}\n\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encountered exception {ex.GetType().Name} while fetching messages:\n\n{ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }
}