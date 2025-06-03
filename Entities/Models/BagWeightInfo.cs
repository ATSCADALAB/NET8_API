using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("BagWeightInfos")]
    public class BagWeightInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public int Bag1 { get; set; }

        [Required]
        public int Bag2 { get; set; }

        [Required]
        public int Bag3 { get; set; }

        [Required]
        public int Bag4 { get; set; }
    }
}
