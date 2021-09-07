using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.Domain
{
    public class KafkaService
    {
        private readonly IProducer<Null, string> producerBuilder;
        private readonly IConsumer<Null, string> consumerBuilder;
        private readonly CancellationTokenSource cts;

        public KafkaService(IConfiguration configuration)
        {
            IReadOnlyDictionary<string, string> producer = AppSettings.AppSettings.GetConfig(configuration, "Producer");
            IReadOnlyDictionary<string, string> consumer = AppSettings.AppSettings.GetConfig(configuration, "Consumer");
            if (producer != null && producer.Any())
            {
                ProducerConfig producerConfig = new()
                {
                    BootstrapServers = producer.TryGetValue("BootStrapServers", out string server) ? server : string.Empty
                };
                producerBuilder = new ProducerBuilder<Null, string>(producerConfig).Build();
            }
            if (consumer != null && consumer.Any())
            {
                ConsumerConfig consumerConfig = new()
                {
                    BootstrapServers = consumer.TryGetValue("BootStrapServers", out string server) ? server : string.Empty,
                    GroupId = consumer.TryGetValue("GroupId", out string groupId) ? groupId : string.Empty,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
                consumerBuilder = new ConsumerBuilder<Null, string>(consumerConfig).Build();
                consumerBuilder.Subscribe(consumer.TryGetValue("TopicName", out string topicName) ? topicName : string.Empty);
                cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };
            }
        }

        public async Task<(DeliveryResult<Null, string>, string)> Publish(string topicName, string data)
        {
            try
            {
                return (await producerBuilder.ProduceAsync(topicName, new Message<Null, string>
                {
                    Value = data
                }), string.Empty);
            }
            catch (ProduceException<Null, string> pex)
            {
                return (default, pex.Error.Reason);
            }
        }

        public (ConsumeResult<Null, string>, string) Subscribe()
        {
            try
            {
                return (consumerBuilder.Consume(cts.Token), string.Empty);
            }
            catch (ConsumeException cex)
            {
                return (default, cex.Error.Reason);
            }
        }
    }
}
