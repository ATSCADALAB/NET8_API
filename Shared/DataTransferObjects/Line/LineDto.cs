namespace Shared.DataTransferObjects.Line
{
    public record LineDto
    {
        public int Id { get; init; }
        public int LineNumber { get; init; }
        public string LineName { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}