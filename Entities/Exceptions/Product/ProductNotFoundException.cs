namespace Entities.Exceptions.Product
{
    public sealed class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(int productId)
            : base($"The product with id: {productId} doesn't exist in the database.")
        {
        }
    }
}