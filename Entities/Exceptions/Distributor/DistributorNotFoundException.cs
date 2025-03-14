namespace Entities.Exceptions.Distributor
{
    public sealed class DistributorNotFoundException : NotFoundException
    {
        public DistributorNotFoundException(int distributorId)
            : base($"The distributor with id: {distributorId} doesn't exist in the database.")
        {
        }
    }
}