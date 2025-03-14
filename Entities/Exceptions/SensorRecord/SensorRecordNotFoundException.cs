namespace Entities.Exceptions.SensorRecord
{
    public sealed class SensorRecordNotFoundException : NotFoundException
    {
        public SensorRecordNotFoundException(int sensorRecordId)
            : base($"The sensor record with id: {sensorRecordId} doesn't exist in the database.")
        {
        }
    }
}