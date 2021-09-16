using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Kafka.Domain.AppSettings
{
    public static class AppSettings
    {
        private const string keyPrefix = "KafkaSettings";

        public static IReadOnlyDictionary<string, string> GetConfig(IConfiguration configuration, string key)
        {
            return configuration.GetSection($"{keyPrefix}:{key}").GetChildren()
                .ToDictionary(k => k.Key, k => k.Value);
        }

        public static string GetTopicName(IConfiguration configuration, string key)
        {
            return GetConfig(configuration, $"Producer:{key}")?.FirstOrDefault(c => c.Key.Equals("TopicName")).Value;
        }
    }
}
