using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.BagWeightInfo
{
    public record BagWeightInfoDto
    {
        public int Id { get; init; }
        public double Weight { get; init; }
        public int Bag1 { get; init; }
        public int Bag2 { get; init; }
        public int Bag3 { get; init; }
        public int Bag4 { get; init; }
    }
}