namespace Kafka.Domain.Models
{
    public enum Status
    {
        OrderSubmitted,
        OrderValidated,
        OrderOutOfStock,
        PaymentProcessed,
        PaymentFailed,
        OrderDispatched
    }
}
