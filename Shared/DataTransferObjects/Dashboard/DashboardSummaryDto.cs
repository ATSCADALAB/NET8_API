namespace Shared.DataTransferObjects.Dashboard
{
    public record DashboardSummaryDto(int TotalOrdersToday, int PendingOrders, int CompletedOrdersToday, int TotalDistributors, int TotalAreas);
    public record OrdersByLineDto(string LineName, int TotalOrders);
    public record OrderStatusTrendDto(string Date, int Pending, int Processing, int Incomplete, int Completed);
    public record TopProductDto(string ProductName, int TotalUnits);
    public record IncompleteOrderDto(string Date, string OrderNumber, string VehicleNumber, string ProductName, int RequestedUnits, int ActualUnits, decimal CompletionPercentage);
    public record ProcessingOrderDto(string Date, string OrderNumber, string VehicleNumber, string LineName, int TotalUnits, string Status);
    public record RecentCompletedOrderDto(string CompletedDate, string OrderNumber, string DistributorName, string ProductName, int TotalUnits);
}