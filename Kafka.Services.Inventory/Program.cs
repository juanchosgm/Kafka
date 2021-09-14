using Confluent.Kafka;
using Kafka.Domain;
using Kafka.Domain.AppSettings;
using Kafka.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kafka.Services.Inventory
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            string producerValidatedTopicName = AppSettings.GetTopicName(configuration, "Validated");
            string producerReportedTopicName = AppSettings.GetTopicName(configuration, "Reported");
            KafkaService kafkaService = new(configuration);
            while (true)
            {
                (ConsumeResult<Null, string> consumerResult, string errorConsumer) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(errorConsumer))
                {
                    Order order = JsonSerializer.Deserialize<Order>(consumerResult.Message.Value);
                    (Report report, bool isValidated) = await DoInventory(order);
                    string reportPayload = JsonSerializer.Serialize(report);
                    (_, _) = await kafkaService.PublishAsync(producerReportedTopicName, reportPayload);
                    if (isValidated)
                    {
                        (_, _) = await kafkaService.PublishAsync(producerValidatedTopicName, consumerResult.Message.Value);
                    }
                }
            }
        }

        private static async Task<(Report, bool)> DoInventory(Order order)
        {
            bool isValidated = false;
            await Task.Delay(1000);
            Report report = new()
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has NOT been validated due to out of stock.",
                Status = Status.OrderOutOfStock,
                CreatedOn = DateTime.UtcNow,
            };
            if (order.Quantity < 6)
            {
                report.Details = "Order has been validated.";
                report.Status = Status.OrderValidated;
                isValidated = true;
            }
            return (report, isValidated);
        }
    }
}
