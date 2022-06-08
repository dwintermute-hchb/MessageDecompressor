using Azure.Messaging.ServiceBus;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Newtonsoft.Json;

namespace MessageDecompressor.Library
{
    public class MessageDecompressorLib
    {
        private static readonly MessagePackSerializerOptions MessagePackSerializerOptions = MessagePackSerializerOptions
            .Standard
            .WithResolver(
                CompositeResolver.Create(
                    NativeDateTimeResolver.Instance,
                    ImmutableCollectionResolver.Instance,
                    ContractlessStandardResolverAllowPrivate.Instance))
            .WithCompression(MessagePackCompression.Lz4BlockArray);

        private const int MaxMessages = 1000;

        public async static Task<IEnumerable<DecompressedItem>> GetDecompressedMessages(string connectionString, string queueName, CancellationToken ct = default)
        {
            var client = new ServiceBusClient(connectionString);

            var receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });

            var messages = await receiver.PeekMessagesAsync(MaxMessages, null, ct);

            if (ct.IsCancellationRequested)
            {
                return new List<DecompressedItem>();
            }

            var decodedMessages = messages.Select(GetJsonFromMessage);

            return decodedMessages;
        }

        private static DecompressedItem GetJsonFromMessage(ServiceBusReceivedMessage message)
        {
            var initialJson = MessagePackSerializer.ConvertToJson(message.Body.ToMemory(), MessagePackSerializerOptions);
            var tempJObject = JsonConvert.DeserializeObject(initialJson);
            var formattedJson = JsonConvert.SerializeObject(tempJObject, Formatting.Indented);

            return new DecompressedItem(message.MessageId, formattedJson);
        }
    }
}