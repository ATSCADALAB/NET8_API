// QuickStart/Service/WcfService.cs
using QuickStart.Entities.Models;
using QuickStart.Service.Contracts;
using QuickStart.Shared.DataTransferObjects.Wcf;
using QuickStart.Utilities;
using Microsoft.AspNetCore.SignalR;
using QuickStart.Hubs;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.DataTransferObjects.Wcf;

namespace QuickStart.Service
{
    public class WcfService : IWcfService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<DataHub> _hubContext;
        private ChannelFactory<IATSCADAService> _channelFactory;
        private IATSCADAService _channel;
        private CancellationTokenSource _cts;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private bool _isPolling;
        private string _address;
        private string _addressIWebAPI;

        public WcfService(IConfiguration configuration, IHubContext<DataHub> hubContext, IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _hubContext = hubContext;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _address = _configuration["WcfService:Address"] ?? "113.161.76.105:9002";
            _addressIWebAPI = _configuration["WcfService:AddressIWebAPI"] ?? "http://113.161.76.105:9006/api/atscada";
            Start();
        }

        public bool IsActive { get; private set; }

        private void Start()
        {
            var username = _configuration["WcfService:Username"] ?? "ATSCADALab___1jbyq8Yg1";
            var password = _configuration["WcfService:Password"] ?? "ATSCADA.Lab.!@#%aajUyqn61HDt";

            var binding = new CustomNetTcpBinding
            {
                OpenTimeout = TimeSpan.FromMinutes(2),
                SendTimeout = TimeSpan.FromMinutes(2),
                ReceiveTimeout = TimeSpan.FromMinutes(10)
            };
            var endpointAddress = new EndpointAddress($"net.tcp://{_address}/ATSCADAService");
            _channelFactory = new ChannelFactory<IATSCADAService>(binding, endpointAddress);
            _channelFactory.Credentials.UserName.UserName = username;
            _channelFactory.Credentials.UserName.Password = password;
            _channel = _channelFactory.CreateChannel();
            IsActive = true;
        }

        public async Task<WcfDataDto[]> ReadTagsAsync(string[] tagNames)
        {
            try
            {
                if (!IsActive) Start();

                var encryptedNames = tagNames.Select(n => n.EncryptAddress()).ToArray();
                var result = await Task.Run(() => _channel.Read(encryptedNames));
                var decryptedResult = result.Decrypt(); // Sử dụng extension method
                return decryptedResult?.Select(r => new WcfDataDto
                {
                    Name = r.Name,
                    Value = r.Value,
                    Status = r.Status,
                    TimeStamp = r.TimeStamp
                }).ToArray() ?? Array.Empty<WcfDataDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read error: {ex.Message}");
                HandleException();
                return Array.Empty<WcfDataDto>();
            }
        }

        public async Task StartPollingAsync(string[] tagNames, int intervalMs)
        {
            if (_isPolling) return;

            _cts = new CancellationTokenSource();
            _isPolling = true;

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var data = await ReadTagsAsync(tagNames);
                    if (data.Any())
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveData", data);
                    }
                    await Task.Delay(intervalMs, _cts.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Polling error: {ex.Message}");
                    HandleException();
                    await Task.Delay(2000, _cts.Token);
                }
            }
        }

        public async Task StopPollingAsync()
        {
            if (!_isPolling) return;
            _cts?.Cancel();
            _isPolling = false;
            await Task.CompletedTask;
        }

        private void HandleException()
        {
            if (_channel is ICommunicationObject commObject &&
                (commObject.State == CommunicationState.Faulted || commObject.State == CommunicationState.Closed))
            {
                IsActive = false;
                commObject.Abort();
                _channelFactory.Close();
                Start();
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            if (_channel is ICommunicationObject commObject)
            {
                commObject.Close();
            }
            _channelFactory.Close();
            IsActive = false;
        }

        public async Task StartResetValue(IEnumerable<WcfDataForUpdateDto> requestList)
        {
            try
            {
                // Lấy token từ cache (đã lưu khi login)
                if (!_cache.TryGetValue("IWebAPIToken", out string? tokenIWebAPI) || string.IsNullOrEmpty(tokenIWebAPI))
                {
                    throw new UnauthorizedAccessException("Token không tồn tại hoặc đã hết hạn.");
                }

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenIWebAPI);

                var response = await httpClient.PutAsJsonAsync($"{_addressIWebAPI}", requestList);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Lỗi cập nhật dữ liệu: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine($"Lỗi trong StartResetValue: {ex.Message}");
                throw; // Ném lại lỗi để Controller xử lý
            }
        }

    }
}