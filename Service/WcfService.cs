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
using Shared.DataTransferObjects.Authentication;
using Contracts;

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
        private readonly IRepositoryManager _repository;

        public WcfService(IConfiguration configuration, IHubContext<DataHub> hubContext, IMemoryCache cache, IHttpClientFactory httpClientFactory, IRepositoryManager repository)
        {
            _configuration = configuration;
            _hubContext = hubContext;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
            _address = _configuration["WcfService:Address"] ?? "10.62.0.21:9002";
            _addressIWebAPI = _configuration["WcfService:AddressIWebAPI"] ?? "http://10.62.0.21:9004/api/atscada";
            _repository = repository;
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
                var decryptedResult = result.Decrypt(); 
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
        public async Task<bool> StartResetValue(IEnumerable<WcfDataForUpdateDto> requestList)
        {
            try
            {
                requestList.First().ValueToWrite = "0";
                var line = requestList.First().Name[requestList.First().Name.Length - 1];
                var confirm = new WcfDataForUpdateDto
                {
                    Name = $"SettingLine{line}.Confirm",
                    ValueToWrite = "0"
                };
                var setting = new WcfDataForUpdateDto
                {
                    Name = $"SettingLine{line}.Setting",
                    ValueToWrite = "0"
                };
                var mutableList = requestList.ToList();
                mutableList.Add(confirm);
                mutableList.Add(setting);

                // Lấy token từ cache hoặc khởi tạo rỗng
                string token = _cache.Get<string>("IWebAPIToken") ?? string.Empty;

                // Hàm hỗ trợ để lấy token mới
                async Task<string> GetNewToken()
                {
                    var userForAuthentication = new UserForAuthenticationDto
                    {
                        UserName = _configuration["APIService:Username"] ?? "atlab",
                        Password = _configuration["APIService:Password"] ?? "atpro1234560"
                    };

                    using var httpClient = _httpClientFactory.CreateClient();
                    var responseToken = await httpClient.PostAsJsonAsync($"{_addressIWebAPI}/login", userForAuthentication);
                    if (responseToken.IsSuccessStatusCode)
                    {
                        var iWebAPIResponse = await responseToken.Content.ReadFromJsonAsync<IWebToken>();
                        if (!string.IsNullOrEmpty(iWebAPIResponse?.Token))
                        {
                            _cache.Set("IWebAPIToken", iWebAPIResponse.Token, TimeSpan.FromHours(29)); // Lưu token, trừ 1 giờ để an toàn
                            return iWebAPIResponse.Token;
                        }
                        throw new Exception("Token iWebAPI rỗng hoặc không hợp lệ.");
                    }
                    var errorMessage = await responseToken.Content.ReadAsStringAsync();
                    throw new Exception($"Không thể lấy token từ iWebAPI: {responseToken.StatusCode} - {errorMessage}");
                }

                // Nếu token rỗng, lấy token mới
                if (string.IsNullOrEmpty(token))
                {
                    token = await GetNewToken();
                }

                // Gửi yêu cầu cập nhật dữ liệu
                using var updateClient = _httpClientFactory.CreateClient();
                updateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await updateClient.PutAsJsonAsync($"{_addressIWebAPI}", mutableList);

                // Kiểm tra nếu token hết hạn (401 Unauthorized)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Lấy token mới và thử lại
                    token = await GetNewToken();
                    updateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    response = await updateClient.PutAsJsonAsync($"{_addressIWebAPI}", mutableList);
                }

                // Kiểm tra kết quả
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi cập nhật dữ liệu: {response.StatusCode} - {errorMessage}");
                    return false; // Trả về false thay vì throw exception
                }

                return true; // Thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong StartResetValue: {ex.Message}");
                return false; // Trả về false thay vì throw exception
            }
        }

        public async Task<bool> StartWriteValue(IEnumerable<WcfDataForUpdateDto> requestList)
        {
            try
            {
                string input = requestList.First().ValueToWrite;
                string[] parts = input.Split('/');
                string result = string.Join("/", parts.Take(3));
                string numberPart = new string(parts[1].Where(char.IsDigit).ToArray());
                var code = new WcfDataForUpdateDto
                {
                    Name = $"SettingLine{requestList.First().Name}.ProductCode",
                    ValueToWrite = numberPart
                };
                var BagWeightInfo = await _repository.BagWeightInfo.GetBagWeightInfoByWeightAsync(double.Parse(parts[parts.Length - 1]), trackChanges: false);

                // Lấy token từ cache hoặc khởi tạo rỗng
                string token = _cache.Get<string>("IWebAPIToken") ?? string.Empty;
                requestList.First().ValueToWrite = result + "/100/" + BagWeightInfo.Bag1 + "/" + BagWeightInfo.Bag2 + "/" + BagWeightInfo.Bag3 + "/" + BagWeightInfo.Bag4;
                requestList.First().Name = $"SettingLine{requestList.First().Name}.Setting";

                var mutableList = requestList.ToList();
                mutableList.Add(code);

                // Hàm hỗ trợ để lấy token mới
                async Task<string> GetNewToken()
                {
                    var userForAuthentication = new UserForAuthenticationDto
                    {
                        UserName = _configuration["APIService:Username"] ?? "atlab",
                        Password = _configuration["APIService:Password"] ?? "atpro1234560"
                    };

                    using var httpClient = _httpClientFactory.CreateClient();
                    var responseToken = await httpClient.PostAsJsonAsync($"{_addressIWebAPI}/login", userForAuthentication);
                    if (responseToken.IsSuccessStatusCode)
                    {
                        var iWebAPIResponse = await responseToken.Content.ReadFromJsonAsync<IWebToken>();
                        if (!string.IsNullOrEmpty(iWebAPIResponse?.Token))
                        {
                            _cache.Set("IWebAPIToken", iWebAPIResponse.Token, TimeSpan.FromHours(29)); // Lưu token, trừ 1 giờ để an toàn
                            return iWebAPIResponse.Token;
                        }
                        throw new Exception("Token iWebAPI rỗng hoặc không hợp lệ.");
                    }
                    var errorMessage = await responseToken.Content.ReadAsStringAsync();
                    throw new Exception($"Không thể lấy token từ iWebAPI: {responseToken.StatusCode} - {errorMessage}");
                }

                // Nếu token rỗng, lấy token mới
                if (string.IsNullOrEmpty(token))
                {
                    token = await GetNewToken();
                }

                // Gửi yêu cầu cập nhật dữ liệu
                using var updateClient = _httpClientFactory.CreateClient();
                updateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await updateClient.PutAsJsonAsync($"{_addressIWebAPI}", mutableList);

                // Kiểm tra nếu token hết hạn (401 Unauthorized)
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Lấy token mới và thử lại
                    token = await GetNewToken();
                    updateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    await updateClient.PutAsJsonAsync($"{_addressIWebAPI}", mutableList);
                    response = await updateClient.PutAsJsonAsync($"{_addressIWebAPI}", mutableList);
                }

                // Kiểm tra kết quả
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi cập nhật dữ liệu: {response.StatusCode} - {errorMessage}");
                    return false; // Trả về false thay vì throw exception
                }

                return true; // Thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong StartWriteValue: {ex.Message}"); // Sửa tên method trong log
                return false; // Trả về false thay vì throw exception
            }
        }
    }
}