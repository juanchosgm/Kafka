using Confluent.Kafka;
using Kafka.Domain;
using Kafka.Domain.AppSettings;
using Kafka.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kafka.Services.Ordering
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            string producerSubmittedTopicName = AppSettings.GetTopicName(configuration, "Submitted");
            string producerReportedTopicName = AppSettings.GetTopicName(configuration, "Reported");
            KafkaService kafkaService = new(configuration);
            while (true)
            {
                (Order order, Report report) = await DoOrdering();
                string reportPayload = JsonSerializer.Serialize(report);
                (_, _) = await kafkaService.PublishAsync(producerReportedTopicName, reportPayload);
                string orderPayload = JsonSerializer.Serialize(order);
                (_, _) = await kafkaService.PublishAsync(producerSubmittedTopicName, orderPayload);
            }
        }

        private static async Task<(Order, Report)> DoOrdering()
        {
            Random random = new();
            await Task.Delay(1000);
            Guid productId = Guid.NewGuid();
            Order order = new()
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductName = $"Product {productId}",
                Quantity = random.Next(1, 10),
                Price = random.Next(1, 100),
                CreatedOn = DateTime.UtcNow,
            };
            Report report = new()
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has been sumiited",
                Status = Status.OrderSubmitted,
                CreatedOn = DateTime.UtcNow
            };
            return (order, report);
        }
    }
}
