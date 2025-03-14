namespace Shared.DataTransferObjects.Area
{
    public abstract record AreaForManipulationDto
    {
        public string AreaCode { get; init; }
        public string AreaName { get; init; }
    }
}