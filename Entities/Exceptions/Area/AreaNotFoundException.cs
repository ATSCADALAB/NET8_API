namespace Entities.Exceptions.Area
{
    public sealed class AreaNotFoundException : NotFoundException
    {
        public AreaNotFoundException(int areaId)
            : base($"The area with id: {areaId} doesn't exist in the database.")
        {
        }
    }
}