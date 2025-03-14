namespace Entities.Exceptions.OrderLineDetail
{
    public sealed class OrderLineDetailNotFoundException : NotFoundException
    {
        public OrderLineDetailNotFoundException(int orderLineDetailId)
            : base($"The order line detail with id: {orderLineDetailId} doesn't exist in the database.")
        {
        }
    }
}