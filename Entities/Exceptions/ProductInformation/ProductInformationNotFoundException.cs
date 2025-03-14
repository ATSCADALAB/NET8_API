namespace Entities.Exceptions.ProductInformation
{
    public sealed class ProductInformationNotFoundException : NotFoundException
    {
        public ProductInformationNotFoundException(int productInformationId)
            : base($"The product information with id: {productInformationId} doesn't exist in the database.")
        {
        }
    }
}