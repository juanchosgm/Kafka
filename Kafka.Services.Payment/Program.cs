using Confluent.Kafka;
using Kafka.Domain;
using Kafka.Domain.AppSettings;
using Kafka.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kafka.Services.Payment
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            string producerProcessedTopicName = AppSettings.GetTopicName(configuration, "Processed");
            string producerReportedTopicName = AppSettings.GetTopicName(configuration, "Reported");
            KafkaService kafkaService = new(configuration);
            while (true)
            {
                (ConsumeResult<Null, string> consumerResult, string errorConsumer) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(errorConsumer))
                {
                    Order order = JsonSerializer.Deserialize<Order>(consumerResult.Message.Value);
                    (Report report, bool isValidated) = await DoPaymentProcess(order);
                    string reportPayload = JsonSerializer.Serialize(report);
                    (_, _) = await kafkaService.PublishAsync(producerReportedTopicName, reportPayload);
                    if (isValidated)
                    {
                        (_, _) = await kafkaService.PublishAsync(producerProcessedTopicName, consumerResult.Message.Value);
                    }
                }
            }
        }

        private static async Task<(Report, bool)> DoPaymentProcess(Order order)
        {
            bool isProcessed = false;
            Console.WriteLine($"Processing Order Id: {order.Id}");
            await Task.Delay(10000);
            Report report = new()
            {
                Id = Guid.NewGuid(),
                Order = order,
                CreatedOn = DateTime.UtcNow,
            };
            if (order.Price > 50)
            {
                report.Details = "Order has not been processed due to failed payment.";
                report.Status = Status.PaymentFailed;
            }
            else
            {
                report.Details = "Product has been processed";
                report.Status = Status.PaymentProcessed;
                isProcessed = true;
            }
            Console.WriteLine($"Payment Process Result: {report.Status}");
            return (report, isProcessed);
        }
    }
}
