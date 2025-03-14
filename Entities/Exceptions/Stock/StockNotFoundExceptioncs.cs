namespace Entities.Exceptions.Stock
{
    public sealed class StockNotFoundException : NotFoundException
    {
        public StockNotFoundException(int stockId)
            : base($"The stock with id: {stockId} doesn't exist in the database.")
        {
        }
    }
}