namespace Entities.Exceptions.Distributor
{
    public class DistributorNotFoundException : NotFoundException
    {
        public DistributorNotFoundException(long Product)
            : base($"Distributor with id: {Product} doesn't exist in the database.")
        {
        }
    }
}
