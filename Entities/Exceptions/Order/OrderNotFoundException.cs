namespace Entities.Exceptions.Order
{
    public sealed class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException(Guid orderID)
            : base($"The order with id: {orderID} doesn't exist in the database.")
        {
        }
    }
}
