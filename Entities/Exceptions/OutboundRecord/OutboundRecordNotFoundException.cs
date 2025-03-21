namespace Entities.Exceptions.OutboundRecord
{
    public sealed class OutboundRecordNotFoundException : NotFoundException
    {
        public OutboundRecordNotFoundException(int OutboundRecordId)
            : base($"The Outbound record with id: {OutboundRecordId} doesn't exist Out the database.")
        {
        }
    }
}