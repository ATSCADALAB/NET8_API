namespace Entities.Exceptions.OrderDetail
{
    public sealed class OrderDetailNotFoundException : NotFoundException
    {
        public OrderDetailNotFoundException(int orderDetailId)
            : base($"The order detail with id: {orderDetailId} doesn't exist in the database.")
        {
        }
    }
}