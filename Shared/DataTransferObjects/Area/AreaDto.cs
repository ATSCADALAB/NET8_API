namespace Shared.DataTransferObjects.Area
{
    public record AreaDto
    {
        public int Id { get; init; }
        public string AreaCode { get; init; }
        public string AreaName { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}