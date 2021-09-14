using Confluent.Kafka;
using Kafka.Domain;
using Kafka.Domain.AppSettings;
using Kafka.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kafka.Services.Dispatch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            string producerReportedTopicName = AppSettings.GetTopicName(configuration, "Reported");
            KafkaService kafkaService = new(configuration);
            while (true)
            {
                (ConsumeResult<Null, string> consumerResult, string errorConsumer) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(errorConsumer))
                {
                    Order order = JsonSerializer.Deserialize<Order>(consumerResult.Message.Value);
                    Report report = await DoDispatch(order);
                    string reportPayload = JsonSerializer.Serialize(report);
                    (_, _) = await kafkaService.PublishAsync(producerReportedTopicName, reportPayload);
                }
            }
        }

        private static async Task<Report> DoDispatch(Order order)
        {
            await Task.Delay(1000);
            return new Report
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has been dispatched",
                Status = Status.OrderDispatched,
                CreatedOn = DateTime.UtcNow,
            };
        }
    }
}
