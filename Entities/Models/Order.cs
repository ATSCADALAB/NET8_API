using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Order
    {
        [Column("OrderID")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; } //Mã Đơn hàng
        public DateTime ExportDate { get; set; } //Ngày xuất hàng
        public int QuantityVehicle {  get; set; } //Số tài xe
        public string VehicleNumber { get; set; } //Biển số xe vận chuyển
        public int ContainerNumber {  get; set; } // Số Cont 
        public int SealNumber {  get; set; } //Số Seal
        public string DriverName { get; set; } //Tên TX
        public string DriverPhoneNumber { get; set; } //SĐT TX
        
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string UnitOrder { get; set; } //Số lượng ( Bao )

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal WeightOrder { get; set; } = 0m; // Số Lượng ( Kg )
        public DateTime ManufactureDate { get; set; } // Ngày sản xuất đơn hàng
        public int Status { get; set; } // Trạng thái đơn hàng
        //
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Ngày tạo đơn hàng
        public DateTime? UpdatedDate { get; set; } // Ngày cập nhật trạng thái

        public string? CreatedBy { get; set; } // Người tạo đơn hàng
        public string? UpdatedBy { get; set; } // Người cập nhật trạng thái
        public ProductInformation? ProductInformation { get; set; } //FK tới Product Infor để lấy thông tin đơn hàng
        [Required(ErrorMessage = "ProductInformation is required")]
        public long ProductInformationID { get; set; }
        public Distributor? Distributor { get; set; } //FK tới Distributor để lấy thông tin nhà cung cấp
        [Required(ErrorMessage = "Distributor is required")]
        public long DistributorID { get; set; }
    }
}
