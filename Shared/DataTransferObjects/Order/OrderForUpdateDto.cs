namespace Shared.DataTransferObjects.Order
{
    public record OrderForUpdateDto : OrderForManipulationDto
    {
        public DateTime UpdatedDate { get; set; } = DateTime.Now; // Ngày cập nhật trạng thái
        public string? UpdatedBy { get; set; } // Người cập nhật trạng thái
    }
}
