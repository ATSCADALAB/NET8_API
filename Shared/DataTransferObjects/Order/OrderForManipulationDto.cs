using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Order
{
    public abstract record OrderForManipulationDto
    {
        public string Code { get; set; } //Mã Đơn hàng
        public DateTime? ExportDate { get; set; } //Ngày xuất hàng
        public int QuantityVehicle { get; set; } //Số tài xe
        public string VehicleNumber { get; set; } //Biển số xe vận chuyển
        public int ContainerNumber { get; set; } // Số Cont 
        public int SealNumber { get; set; } //Số Seal
        public string DriverName { get; set; } //Tên TX
        public string DriverPhoneNumber { get; set; } //SĐT TX

        public string ProductCode { get; set; }
        public string? UnitOrder { get; set; } //Số lượng ( Bao )
        public decimal WeightOrder { get; set; } = 0m; // Số Lượng ( Kg )
        public DateTime? ManufactureDate { get; set; } // Ngày sản xuất đơn hàng
        public bool Status { get; set; } = false; // Trạng thái đơn hàng

        public long ProductInformationID { get; set; }
        public long DistributorID {  get; set; }

    }
}
