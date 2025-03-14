namespace Entities.Exceptions.InboundRecord
{
    public sealed class InboundRecordNotFoundException : NotFoundException
    {
        public InboundRecordNotFoundException(int inboundRecordId)
            : base($"The inbound record with id: {inboundRecordId} doesn't exist in the database.")
        {
        }
    }
}