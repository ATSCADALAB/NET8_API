namespace Shared.DataTransferObjects.Order
{
    public record OrderForCreationDto : OrderForManipulationDto
    {
        public string? CreatedBy { get; set; } // Người tạo đơn hàng
    }
}
