namespace Entities.Exceptions.Product
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(long Product)
            : base($"Product with id: {Product} doesn't exist in the database.")
        {
        }
    }
}
