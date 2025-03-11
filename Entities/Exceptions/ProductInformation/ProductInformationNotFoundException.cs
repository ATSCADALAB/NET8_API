namespace Entities.Exceptions.ProductInformation
{
    public class ProductInformationNotFoundException : NotFoundException
    {
        public ProductInformationNotFoundException(long Product)
            : base($"ProductInformation with id: {Product} doesn't exist in the database.")
        {
        }
    }
}
