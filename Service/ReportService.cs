using AutoMapper;
using Contracts; // Giả định ILoggerManager nằm trong namespace này
using Service.Contracts;
using Shared.DataTransferObjects.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.IO;
using ClosedXML.Excel;

namespace Service
{
    internal sealed class ReportService : IReportService
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly string _connectionString;

        public ReportService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _connectionString = configuration.GetConnectionString("sqlConnection");
        }

        public async Task<IEnumerable<ProductDailyReportDto>> GetProductDailyReportAsync(
            DateTime startDate,
            DateTime endDate,
            int? lineId,
            int? productInformationId)
        {
            _logger.LogInfo($"Fetching Product Daily Report from {startDate} to {endDate}, LineId: {lineId}, ProductId: {productInformationId}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetProductDailyReport";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new MySqlParameter("@p_start_date", startDate));
                        command.Parameters.Add(new MySqlParameter("@p_end_date", endDate));
                        command.Parameters.Add(new MySqlParameter("@p_line_id", lineId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_product_information_id", productInformationId ?? (object)DBNull.Value));

                        var reports = new List<ProductDailyReportDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reports.Add(new ProductDailyReportDto
                                {
                                    Date = reader.GetString("Date"),
                                    LineName = reader.GetString("LineName"),
                                    ProductName = reader.GetString("ProductName"),
                                    TotalOrders = reader.GetInt32("TotalOrders"),
                                    TotalSensorWeight = reader.GetDecimal("TotalSensorWeight"),
                                    TotalSensorUnits = reader.GetInt32("TotalSensorUnits")
                                });
                            }
                        }

                        // Nếu không có dữ liệu, trả về null
                        if (!reports.Any())
                        {
                            _logger.LogInfo("No data found for Product Daily Report.");
                            return null;
                        }

                        // Nếu không có lineId và productInformationId -> gom nhóm theo ngày
                        if (!lineId.HasValue && !productInformationId.HasValue)
                        {
                            reports = reports
                                .GroupBy(r => r.Date)
                                .Select(g => new ProductDailyReportDto
                                {
                                    Date = g.Key,
                                    LineName = "All",
                                    ProductName = "All",
                                    TotalOrders = g.Sum(r => r.TotalOrders),
                                    TotalSensorWeight = g.Sum(r => r.TotalSensorWeight),
                                    TotalSensorUnits = g.Sum(r => r.TotalSensorUnits)
                                })
                                .ToList();
                        }

                        return reports;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Product Daily Report: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportProductDailyReportAsync(DateTime startDate, DateTime endDate, int? lineId, int? productInformationId)
        {
            var data = await GetProductDailyReportAsync(startDate, endDate, lineId, productInformationId);
            if (data == null)
            {
                _logger.LogInfo("No data to export for Product Daily Report.");
                return null; // Trả về null nếu không có dữ liệu hoặc lỗi
            }

            string templatePath;
            if (!lineId.HasValue && !productInformationId.HasValue)
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "XuatBaoCaoTongDonHangDaXuat.xlsx");
            }
            else
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "XuatBaoCaoSanPhamQuaBoDem.xlsx");
            }

            try
            {
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

                    // Fill thông tin thời gian
                    worksheet.Cell("C3").Value = $"Từ ngày: {startDate:dd/MM/yyyy}";
                    if (!lineId.HasValue && !productInformationId.HasValue)
                    {
                        worksheet.Cell("D3").Value = $"đến ngày: {endDate:dd/MM/yyyy}";
                    }
                    else
                    {
                        worksheet.Cell("E3").Value = $"đến ngày: {endDate:dd/MM/yyyy}";
                    }

                    // Fill dữ liệu
                    int currentRow = 6;
                    foreach (var item in data)
                    {
                        if (!lineId.HasValue && !productInformationId.HasValue)
                        {
                            worksheet.Cell(currentRow, 1).Value = DateTime.Parse(item.Date).ToString("dd/MM/yyyy");
                            worksheet.Cell(currentRow, 2).Value = item.LineName;
                            worksheet.Cell(currentRow, 3).Value = item.TotalOrders;
                            worksheet.Cell(currentRow, 4).Value = item.TotalSensorUnits;
                            worksheet.Cell(currentRow, 5).Value = item.TotalSensorWeight;
                            var range = worksheet.Range(currentRow, 1, currentRow, 5);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(currentRow, 3, currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        else
                        {
                            worksheet.Cell(currentRow, 1).Value = DateTime.Parse(item.Date).ToString("dd/MM/yyyy");
                            worksheet.Cell(currentRow, 2).Value = item.LineName;
                            worksheet.Cell(currentRow, 3).Value = item.ProductName;
                            worksheet.Cell(currentRow, 4).Value = item.TotalOrders;
                            worksheet.Cell(currentRow, 5).Value = item.TotalSensorUnits;
                            worksheet.Cell(currentRow, 6).Value = item.TotalSensorWeight;
                            var range = worksheet.Range(currentRow, 1, currentRow, 6);
                            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(currentRow, 4, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        // Format số
                        for (int col = 3; col <= (lineId.HasValue || productInformationId.HasValue ? 6 : 5); col++)
                        {
                            worksheet.Cell(currentRow, col).Style.NumberFormat.Format = "#,##0";
                        }

                        currentRow++;
                    }

                    // Dòng tổng cộng
                    int totalRow = currentRow;
                    worksheet.Cell(totalRow, 1).Value = "Tổng cộng";
                    if (!lineId.HasValue && !productInformationId.HasValue)
                    {
                        worksheet.Cell(totalRow, 3).FormulaA1 = $"=SUM(C6:C{currentRow - 1})";
                        worksheet.Cell(totalRow, 4).FormulaA1 = $"=SUM(D6:D{currentRow - 1})";
                        worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E6:E{currentRow - 1})";
                        var totalRange = worksheet.Range(totalRow, 1, totalRow, 5);
                        totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                    }
                    else
                    {
                        worksheet.Cell(totalRow, 4).FormulaA1 = $"=SUM(D6:D{currentRow - 1})";
                        worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E6:E{currentRow - 1})";
                        worksheet.Cell(totalRow, 6).FormulaA1 = $"=SUM(F6:F{currentRow - 1})";
                        var totalRange = worksheet.Range(totalRow, 1, totalRow, 6);
                        totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                    }

                    // Format dòng tổng
                    for (int col = (lineId.HasValue || productInformationId.HasValue ? 4 : 3); col <= (lineId.HasValue || productInformationId.HasValue ? 6 : 5); col++)
                    {
                        worksheet.Cell(totalRow, col).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(totalRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Convert to byte array
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Product Daily Report: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ProductDailyReportDto>> GetVehicleDailyReportAsync(
            DateTime startDate,
            DateTime endDate,
            string licensePlate = null)
        {
            _logger.LogInfo($"Fetching Vehicle Daily Report from {startDate} to {endDate}, LicensePlate: {licensePlate}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetVehicleDailyReport";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new MySqlParameter("@p_start_date", startDate));
                        command.Parameters.Add(new MySqlParameter("@p_end_date", endDate));
                        command.Parameters.Add(new MySqlParameter("@p_license_plate", licensePlate ?? (object)DBNull.Value));

                        var reports = new List<ProductDailyReportDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reports.Add(new ProductDailyReportDto
                                {
                                    Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? "" : reader.GetString("Date"),
                                    LineName = reader.GetString("LineName"),
                                    ProductName = reader.GetString("ProductName"),
                                    TotalOrders = reader.GetInt32("TotalOrders"),
                                    TotalSensorWeight = reader.GetDecimal("TotalSensorWeight"),
                                    TotalSensorUnits = reader.GetInt32("TotalSensorUnits")
                                });
                            }
                        }

                        // Nếu không có dữ liệu, trả về null
                        if (!reports.Any())
                        {
                            _logger.LogInfo("No data found for Vehicle Daily Report.");
                            return null;
                        }

                        return reports;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Vehicle Daily Report: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportVehicleDailyReportAsync(DateTime startDate, DateTime endDate, string licensePlate = null)
        {
            var data = await GetVehicleDailyReportAsync(startDate, endDate, licensePlate);
            if (data == null)
            {
                _logger.LogInfo("No data to export for Vehicle Daily Report.");
                return null; // Trả về null nếu không có dữ liệu hoặc lỗi
            }

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "XuatBaoCaoTheoPhuongTien.xlsx");

            try
            {
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

                    // Fill thông tin thời gian
                    worksheet.Cell("C3").Value = $"Từ ngày: {startDate:dd/MM/yyyy}";
                    worksheet.Cell("D3").Value = $"đến ngày: {endDate:dd/MM/yyyy}";
                    worksheet.Cell("B5").Value = $"{licensePlate}";

                    // Fill dữ liệu từ row 7
                    int currentRow = 7;
                    foreach (var item in data)
                    {
                        worksheet.Cell(currentRow, 1).Value = DateTime.Parse(item.Date).ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = item.LineName;
                        worksheet.Cell(currentRow, 3).Value = item.TotalOrders;
                        worksheet.Cell(currentRow, 4).Value = item.TotalSensorUnits;
                        worksheet.Cell(currentRow, 5).Value = item.TotalSensorWeight;

                        // Format số
                        worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0";

                        // Border
                        var range = worksheet.Range(currentRow, 1, currentRow, 5);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        // Căn giữa số liệu
                        worksheet.Range(currentRow, 3, currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;
                    }

                    // Dòng tổng cộng
                    int totalRow = currentRow;
                    worksheet.Cell(totalRow, 1).Value = "Tổng cộng";
                    worksheet.Cell(totalRow, 3).FormulaA1 = $"=SUM(C6:C{currentRow - 1})";
                    worksheet.Cell(totalRow, 4).FormulaA1 = $"=SUM(D6:D{currentRow - 1})";
                    worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E6:E{currentRow - 1})";

                    // Format dòng tổng
                    var totalRange = worksheet.Range(totalRow, 1, totalRow, 5);
                    totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                    worksheet.Cell(totalRow, 3).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Convert to byte array
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Vehicle Daily Report: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ProductDailyReportDto>> GetIncompleteOrderShipmentReportAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInfo($"Fetching Incomplete Order Shipment Report from {startDate} to {endDate}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetIncompleteOrderShipmentReport";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new MySqlParameter("@p_start_date", startDate));
                        command.Parameters.Add(new MySqlParameter("@p_end_date", endDate));

                        var reports = new List<ProductDailyReportDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reports.Add(new ProductDailyReportDto
                                {
                                    Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? "" : reader.GetString("Date"),
                                    LicensePlate = reader.GetString("LicensePlate"),
                                    LineName = "",
                                    ProductName = reader.GetString("ProductName"),
                                    TotalOrders = 0,
                                    TotalSensorWeight = reader.GetDecimal("ActualWeight"),
                                    TotalSensorUnits = reader.GetInt32("ActualUnits"),
                                    TotalWeightToDate = reader.GetDecimal("ActualWeight"),
                                    TotalUnitsToDate = reader.GetInt32("ActualUnits"),
                                    DailyWeight = 0m,
                                    DailyUnits = 0,
                                    RequestedUnits = reader.GetInt32("RequestedUnits"),
                                    RequestedWeight = reader.GetDecimal("RequestedWeight"),
                                    ActualUnits = reader.GetInt32("ActualUnits"),
                                    ActualWeight = reader.GetDecimal("ActualWeight"),
                                    CompletionPercentage = reader.GetDecimal("CompletionPercentage")
                                });
                            }
                        }

                        // Nếu không có dữ liệu, trả về null
                        if (!reports.Any())
                        {
                            _logger.LogInfo("No data found for Incomplete Order Shipment Report.");
                            return null;
                        }

                        return reports;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Incomplete Order Shipment Report: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportIncompleteOrderShipmentReportAsync(DateTime startDate, DateTime endDate)
        {
            var data = await GetIncompleteOrderShipmentReportAsync(startDate, endDate);
            if (data == null)
            {
                _logger.LogInfo("No data to export for Incomplete Order Shipment Report.");
                return null; // Trả về null nếu không có dữ liệu hoặc lỗi
            }

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "XuatBaoCaoHangConDangDo.xlsx");

            try
            {
                using (var workbook = new XLWorkbook(templatePath))
                {
                    var worksheet = workbook.Worksheet(1); // Lấy sheet đầu tiên

                    // Fill thông tin thời gian
                    worksheet.Cell("C3").Value = $"Từ ngày: {startDate:dd/MM/yyyy}";
                    worksheet.Cell("E3").Value = $"đến ngày: {endDate:dd/MM/yyyy}";

                    // Fill dữ liệu từ row 6
                    int currentRow = 6;
                    foreach (var item in data)
                    {
                        worksheet.Cell(currentRow, 1).Value = DateTime.Parse(item.Date).ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = item.LicensePlate;
                        worksheet.Cell(currentRow, 3).Value = item.ProductName;
                        worksheet.Cell(currentRow, 4).Value = item.RequestedUnits;
                        worksheet.Cell(currentRow, 5).Value = item.TotalSensorUnits;
                        worksheet.Cell(currentRow, 6).Value = item.CompletionPercentage;

                        // Format số
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0";

                        // Border
                        var range = worksheet.Range(currentRow, 1, currentRow, 6);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        // Căn giữa số liệu
                        worksheet.Range(currentRow, 4, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;
                    }

                    // Dòng tổng cộng
                    int totalRow = currentRow;
                    worksheet.Cell(totalRow, 1).Value = "Tổng cộng";
                    worksheet.Cell(totalRow, 4).FormulaA1 = $"=SUM(D6:D{currentRow - 1})";
                    worksheet.Cell(totalRow, 5).FormulaA1 = $"=SUM(E6:E{currentRow - 1})";
                    worksheet.Cell(totalRow, 6).FormulaA1 = $"=SUM(F6:F{currentRow - 1})";

                    // Format dòng tổng
                    var totalRange = worksheet.Range(totalRow, 1, totalRow, 6);
                    totalRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    totalRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                    worksheet.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 6).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Convert to byte array
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Incomplete Order Shipment Report: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<AgentProductionReportDto>> GetAgentProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null, int? distributorId = null, int? productInformationId = null)
        {
            _logger.LogInfo($"Fetching Distributor Production Report for fromYear {fromYear}, toYear {toYear}, fromMonth {fromMonth}, toMonth {toMonth}, distributorId {distributorId}, productInformationId {productInformationId}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetAgentProductionReport";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new MySqlParameter("@p_from_year", fromYear ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_to_year", toYear ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_from_month", fromMonth ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_to_month", toMonth ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_distributor_id", distributorId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_product_information_id", productInformationId ?? (object)DBNull.Value));

                        var reports = new List<AgentProductionReportDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reports.Add(new AgentProductionReportDto
                                {
                                    DistributorName = reader.GetString("DistributorName"),
                                    ProductName = reader.GetString("ProductName"),
                                    ExportPeriod = reader.GetString("ExportPeriod"),
                                    CumulativeUnits = reader.GetInt32("CumulativeUnits"),
                                    CumulativeWeight = reader.GetDecimal("CumulativeWeight")
                                });
                            }
                        }

                        // Nếu không có dữ liệu, trả về null
                        if (!reports.Any())
                        {
                            _logger.LogInfo("No data found for Distributor Production Report.");
                            return null;
                        }

                        return reports;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Distributor Production Report: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportAgentProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null, int? distributorId = null, int? productInformationId = null)
        {
            var data = await GetAgentProductionReportAsync(fromYear, toYear, fromMonth, toMonth, distributorId, productInformationId);
            if (data == null)
            {
                _logger.LogInfo("No data to export for Distributor Production Report.");
                return null; // Trả về null nếu không có dữ liệu hoặc lỗi
            }

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Distributor Production Report");

                    // Tiêu đề công ty
                    worksheet.Cell("A2").Value = "CÔNG TY CỔ PHẦN PHÂN BÓN HÀ LAN";
                    worksheet.Cell("A2").Style.Font.Bold = true;
                    worksheet.Cell("A2").Style.Font.FontSize = 14;
                    worksheet.Cell("A2").Style.Font.FontColor = XLColor.Red;

                    // Tiêu đề báo cáo
                    worksheet.Cell("A3").Value = "BÁO CÁO SẢN LƯỢNG NHÀ PHÂN PHỐI THEO LUỸ KẾT";
                    worksheet.Cell("A3").Style.Font.Bold = true;
                    worksheet.Cell("A3").Style.Font.FontSize = 12;

                    // Dòng thông tin thời gian (nếu có)
                    int currentHeaderRow = 4;
                    if (fromYear.HasValue && toYear.HasValue && fromMonth.HasValue && toMonth.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ: {fromMonth:D2}/{fromYear} Đến: {toMonth:D2}/{toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (fromYear.HasValue && toYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ Năm: {fromYear} Đến Năm: {toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (fromYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ Năm: {fromYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (toYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Đến Năm: {toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }

                    // Thêm thông tin lọc (nếu có)
                    if (distributorId.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Nhà Phân Phối: {distributorId.Value}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    if (productInformationId.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Sản Phẩm: {productInformationId.Value}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }

                    // Dòng trống để căn chỉnh
                    worksheet.Row(currentHeaderRow + 1).Height = 20;

                    // Tiêu đề cột
                    var headerRow = worksheet.Row(currentHeaderRow + 2);
                    headerRow.Cell(1).Value = "Nhà Phân Phối";
                    headerRow.Cell(2).Value = "Sản Phẩm";
                    headerRow.Cell(3).Value = "Tháng/Năm";
                    headerRow.Cell(4).Value = "Số Lượng (Bao)";
                    headerRow.Cell(5).Value = "Trọng Lượng (kg)";

                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Ghi dữ liệu
                    int currentRow = currentHeaderRow + 3;
                    foreach (var item in data)
                    {
                        worksheet.Cell(currentRow, 1).Value = item.DistributorName;
                        worksheet.Cell(currentRow, 2).Value = item.ProductName;
                        worksheet.Cell(currentRow, 3).Value = item.ExportPeriod;
                        worksheet.Cell(currentRow, 4).Value = item.CumulativeUnits;
                        worksheet.Cell(currentRow, 5).Value = item.CumulativeWeight;

                        // Định dạng số
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00"; // Trọng lượng với 2 chữ số thập phân

                        // Căn giữa các cột số
                        var range = worksheet.Range(currentRow, 1, currentRow, 5);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(currentRow, 3, currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;
                    }

                    // Điều chỉnh độ rộng cột
                    worksheet.Columns().AdjustToContents();

                    // Xuất file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Distributor Production Report: {ex.Message}");
                return null;
            }
        }
        public async Task<IEnumerable<RegionProductionReportDto>> GetRegionProductionReportAsync(
    int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null,
    int? productInformationId = null, int? areaId = null)
        {
            _logger.LogInfo($"Fetching Region Production Report for fromYear {fromYear}, toYear {toYear}, fromMonth {fromMonth}, toMonth {toMonth}, productInformationId {productInformationId}, areaId {areaId}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "GetRegionProductionReport";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new MySqlParameter("@p_from_year", fromYear ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_to_year", toYear ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_from_month", fromMonth ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_to_month", toMonth ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_product_information_id", productInformationId ?? (object)DBNull.Value));
                        command.Parameters.Add(new MySqlParameter("@p_area_id", areaId ?? (object)DBNull.Value));

                        var reports = new List<RegionProductionReportDto>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reports.Add(new RegionProductionReportDto
                                {
                                    AreaName = reader.GetString("AreaName"),
                                    ProductName = reader.GetString("ProductName"),
                                    ExportPeriod = reader.GetString("ExportPeriod"),
                                    TotalUnits = reader.GetInt32("TotalUnits"),
                                    TotalWeight = reader.GetDecimal("TotalWeight"),
                                    CumulativeUnits = reader.GetInt32("CumulativeUnits"),
                                    CumulativeWeight = reader.GetDecimal("CumulativeWeight")
                                });
                            }
                        }

                        // Nếu không có dữ liệu, trả về null
                        if (!reports.Any())
                        {
                            _logger.LogInfo("No data found for Region Production Report.");
                            return null;
                        }

                        return reports;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching Region Production Report: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportRegionProductionReportAsync(
            int? fromYear = null, int? toYear = null, int? fromMonth = null, int? toMonth = null,
            int? productInformationId = null, int? areaId = null)
        {
            var data = await GetRegionProductionReportAsync(fromYear, toYear, fromMonth, toMonth, productInformationId, areaId);
            if (data == null)
            {
                _logger.LogInfo("No data to export for Region Production Report.");
                return null; // Trả về null nếu không có dữ liệu hoặc lỗi
            }

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Region Production Report");

                    // Tiêu đề công ty
                    worksheet.Cell("A2").Value = "CÔNG TY CỔ PHẦN PHÂN BÓN HÀ LAN";
                    worksheet.Cell("A2").Style.Font.Bold = true;
                    worksheet.Cell("A2").Style.Font.FontSize = 14;
                    worksheet.Cell("A2").Style.Font.FontColor = XLColor.Red;

                    // Tiêu đề báo cáo
                    worksheet.Cell("A3").Value = "BÁO CÁO SẢN LƯỢNG THEO KHU VỰC THEO LUỸ KẾT";
                    worksheet.Cell("A3").Style.Font.Bold = true;
                    worksheet.Cell("A3").Style.Font.FontSize = 12;

                    // Dòng thông tin thời gian và khu vực (nếu có)
                    int currentHeaderRow = 4;
                    if (fromYear.HasValue && toYear.HasValue && fromMonth.HasValue && toMonth.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ: {fromMonth:D2}/{fromYear} Đến: {toMonth:D2}/{toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (fromYear.HasValue && toYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ Năm: {fromYear} Đến Năm: {toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (fromYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Từ Năm: {fromYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }
                    else if (toYear.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Đến Năm: {toYear}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }

                    if (areaId.HasValue)
                    {
                        // Có thể thêm logic để lấy AreaName từ AreaId nếu cần (ở đây tạm thời hiển thị ID)
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Khu Vực ID: {areaId.Value}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }

                    // Thêm thông tin lọc sản phẩm (nếu có)
                    if (productInformationId.HasValue)
                    {
                        worksheet.Cell($"A{currentHeaderRow}").Value = $"Sản Phẩm ID: {productInformationId.Value}";
                        worksheet.Cell($"A{currentHeaderRow}").Style.Font.FontSize = 10;
                        currentHeaderRow++;
                    }

                    // Dòng trống để căn chỉnh
                    worksheet.Row(currentHeaderRow + 1).Height = 20;

                    // Tiêu đề cột
                    var headerRow = worksheet.Row(currentHeaderRow + 2);
                    headerRow.Cell(1).Value = "Khu Vực";
                    headerRow.Cell(2).Value = "Sản Phẩm";
                    headerRow.Cell(3).Value = "Tháng/Năm";
                    headerRow.Cell(4).Value = "Số Lượng (Bao)";
                    headerRow.Cell(5).Value = "Trọng Lượng (kg)";
                    headerRow.Cell(6).Value = "Luỹ Kế Số Lượng (Bao)";
                    headerRow.Cell(7).Value = "Luỹ Kế Trọng Lượng (kg)";

                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Ghi dữ liệu
                    int currentRow = currentHeaderRow + 3;
                    foreach (var item in data)
                    {
                        // Dòng dữ liệu tháng
                        worksheet.Cell(currentRow, 1).Value = item.AreaName;
                        worksheet.Cell(currentRow, 2).Value = item.ProductName;
                        worksheet.Cell(currentRow, 3).Value = item.ExportPeriod;
                        worksheet.Cell(currentRow, 4).Value = item.TotalUnits;
                        worksheet.Cell(currentRow, 5).Value = item.TotalWeight;
                        worksheet.Cell(currentRow, 6).Value = item.CumulativeUnits;
                        worksheet.Cell(currentRow, 7).Value = item.CumulativeWeight;

                        // Định dạng số
                        worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Cell(currentRow, 6).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";

                        // Căn giữa các cột số
                        var range = worksheet.Range(currentRow, 1, currentRow, 7);
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(currentRow, 4, currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;
                    }

                    // Điều chỉnh độ rộng cột
                    worksheet.Columns().AdjustToContents();

                    // Xuất file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting Region Production Report: {ex.Message}");
                return null;
            }
        }
    }
}