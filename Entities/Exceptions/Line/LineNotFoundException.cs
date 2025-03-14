namespace Entities.Exceptions.Line
{
    public sealed class LineNotFoundException : NotFoundException
    {
        public LineNotFoundException(int lineId)
            : base($"The line with id: {lineId} doesn't exist in the database.")
        {
        }
    }
}