namespace Shared.DataTransferObjects.Line
{
    public abstract record LineForManipulationDto
    {
        public int LineNumber { get; init; }
        public string LineName { get; init; }
        public bool IsActive { get; init; } = true;
    }
}