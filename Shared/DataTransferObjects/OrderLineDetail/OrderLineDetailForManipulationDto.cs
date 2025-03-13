namespace Shared.DataTransferObjects
{
    public abstract record OrderLineDetailForManipulationDto
    {
        public Guid OrderId { get; set; } // OrderID
        public int Line { get; set; } // Line
    }
}