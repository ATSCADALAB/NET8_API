namespace Shared.DataTransferObjects.Dashboard
{
    public record DashboardSummaryDto(int TotalOrdersToday, int PendingOrders, int CompletedOrdersToday, int TotalDistributors, int TotalAreas);
    public record OrdersByLineDto(string LineName, int TotalOrders, string OrderID);
    public record OrderStatusTrendDto(string Date, int Pending, int Processing, int Incomplete, int Completed);
    public record TopProductDto(string ProductName, int TotalUnits);
    public record IncompleteOrderDto(string Date, string OrderNumber, string VehicleNumber, string ProductName, int RequestedUnits, int ActualUnits, decimal CompletionPercentage,string OrderId);
    public record ProcessingOrderDto(string Date, string OrderNumber, string VehicleNumber, string LineName, int TotalUnits, int TotalRequestedUnits, string OrderId,string Status);
    public record RecentCompletedOrderDto(string OrderId,string CompletedDate, string OrderNumber, string DistributorName, string ProductName, int TotalUnits);
}