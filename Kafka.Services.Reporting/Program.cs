using Confluent.Kafka;
using Kafka.Domain;
using Kafka.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

namespace Kafka.Services.Reporting
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            KafkaService kafkaService = new(configuration);
            while (true)
            {
                (ConsumeResult<Null, string> consumerResult, string errorConsumer) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(errorConsumer))
                {
                    Report report = JsonSerializer.Deserialize<Report>(consumerResult.Message.Value);
                    Console.WriteLine($"[Report Status: {report.Status}] => [ CreateOn: {report.CreatedOn}, Report Id: {report.Id}, " +
                        $"Order Id: {report.Order.Id}, Report Details: {report.Details}]");
                }
            }
        }
    }
}
