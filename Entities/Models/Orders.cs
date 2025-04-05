using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string OrderCode { get; set; } // Có thể trùng

        public DateTime ExportDate { get; set; }

        [StringLength(20)]
        public string VehicleNumber { get; set; }

        [StringLength(100)]
        public string DriverName { get; set; }

        [StringLength(20)]
        public string DriverPhoneNumber { get; set; }
        public int Status { get; set; }
        public int DriverNumber { get; set; } // 0: Chưa xử lý, 1: Đang xử lý, 2: Hoàn thành, 3: Dở dang

        [Required]
        public int DistributorId { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Distributor (many-to-one)
        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor Distributor { get; set; }

        // Mối quan hệ với OrderDetail (one-to-many)
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Mối quan hệ với OrderLineDetail (one-to-many)
        public virtual ICollection<OrderLineDetail> OrderLineDetails { get; set; } = new List<OrderLineDetail>();

        // Mối quan hệ với SensorRecord (one-to-many)
        public virtual ICollection<SensorRecord> SensorRecords { get; set; } = new List<SensorRecord>();
    }
}