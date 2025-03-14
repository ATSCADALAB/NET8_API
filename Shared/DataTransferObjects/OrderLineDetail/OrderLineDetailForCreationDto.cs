namespace Shared.DataTransferObjects.OrderLineDetail
{
    public record OrderLineDetailForCreationDto : OrderLineDetailForManipulationDto
    {
        public new Guid OrderId { get; set; } // Cho phép set để gán OrderId
    }
}