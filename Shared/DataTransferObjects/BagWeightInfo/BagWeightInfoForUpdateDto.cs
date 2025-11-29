using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.BagWeightInfo
{
    public record BagWeightInfoForUpdateDto
    {
        [Required(ErrorMessage = "Weight is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public double Weight { get; init; }

        [Required(ErrorMessage = "Bag1 is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Bag1 must be non-negative")]
        public int Bag1 { get; init; }

        [Required(ErrorMessage = "Bag2 is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Bag2 must be non-negative")]
        public int Bag2 { get; init; }

        [Required(ErrorMessage = "Bag3 is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Bag3 must be non-negative")]
        public int Bag3 { get; init; }

        [Required(ErrorMessage = "Bag4 is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Bag4 must be non-negative")]
        public int Bag4 { get; init; }
        public int LineID { get; init; }
    }
}