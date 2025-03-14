using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("Areas")]
    [Index(nameof(AreaCode), IsUnique = true)] // Đảm bảo AreaCode duy nhất
    public class Area
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string AreaCode { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string AreaName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Distributor (one-to-many)
        public virtual ICollection<Distributor> Distributors { get; set; } = new List<Distributor>();
    }
}