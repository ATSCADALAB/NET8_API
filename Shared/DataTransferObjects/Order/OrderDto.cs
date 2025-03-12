using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DataTransferObjects.Order
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } //Mã Đơn hàng
        public DateTime ExportDate { get; set; } //Ngày xuất hàng
        public int QuantityVehicle { get; set; } //Số tài xe
        public string VehicleNumber { get; set; } //Biển số xe vận chuyển
        public int ContainerNumber { get; set; } // Số Cont 
        public int SealNumber { get; set; } //Số Seal
        public string DriverName { get; set; } //Tên TX
        public string DriverPhoneNumber { get; set; } //SĐT TX

        public long ProductInformationID { get; set; }
        public string ProductCode { get; set; } //Mã SP
        public string ProductName {  get; set; } //Tên SP
        public long DistributorID { get; set; }
        public string DistributorName { get; set; } //Tên Đại Lý
        public string DistributorArea { get; set; } //Khu vực Đại Lý
        public string UnitOrder { get; set; } //Số lượng ( Bao )
        public decimal WeightOrder { get; set; } = 0m; // Số Lượng ( Kg )
        public DateTime ManufactureDate { get; set; } // Ngày sản xuất đơn hàng
        
        public bool Status { get; set; } = false; // Trạng thái đơn hàng
        //
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Ngày tạo đơn hàng
        public DateTime? UpdatedDate { get; set; } // Ngày cập nhật trạng thái

        public string? CreatedBy { get; set; } // Người tạo đơn hàng
        public string? UpdatedBy { get; set; } // Người cập nhật trạng thái
    }
}
