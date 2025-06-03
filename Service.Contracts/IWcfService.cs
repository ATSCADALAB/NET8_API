// QuickStart/Contracts/IWcfService.cs
using QuickStart.Shared.DataTransferObjects.Wcf;
using Shared.DataTransferObjects.Wcf;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IWcfService
    {
        Task<WcfDataDto[]> ReadTagsAsync(string[] tagNames);
        Task StartPollingAsync(string[] tagNames, int intervalMs); // Đọc liên tục và gửi qua SignalR
        Task<bool> StartResetValue(IEnumerable<WcfDataForUpdateDto> requestList);
        Task<bool> StartWriteValue(IEnumerable<WcfDataForUpdateDto> requestList);
        Task StopPollingAsync();
    }
}