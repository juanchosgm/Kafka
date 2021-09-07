using System;

namespace Kafka.Domain.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
