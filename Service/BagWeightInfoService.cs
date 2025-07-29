using AutoMapper;
using Contracts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Entities.Exceptions;
using Entities.Exceptions.Stock;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.BagWeightInfo;

namespace Service
{
    internal sealed class BagWeightInfoService : IBagWeightInfoService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public BagWeightInfoService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BagWeightInfoDto>> GetAllBagWeightInfosAsync(bool trackChanges)
        {
            var bagWeightInfos = await _repository.BagWeightInfo.GetAllBagWeightInfosAsync(trackChanges);
            var bagWeightInfosDto = _mapper.Map<IEnumerable<BagWeightInfoDto>>(bagWeightInfos);
            return bagWeightInfosDto;
        }

        public async Task<BagWeightInfoDto> GetBagWeightInfoAsync(int bagWeightInfoId, bool trackChanges)
        {
            var bagWeightInfo = await GetBagWeightInfoAndCheckIfItExists(bagWeightInfoId, trackChanges);
            var bagWeightInfoDto = _mapper.Map<BagWeightInfoDto>(bagWeightInfo);
            return bagWeightInfoDto;
        }

        public async Task<BagWeightInfoDto> GetBagWeightInfoByWeightAsync(double weight, bool trackChanges)
        {
            var bagWeightInfo = await _repository.BagWeightInfo.GetBagWeightInfoByWeightAsync(weight, trackChanges);
            if (bagWeightInfo == null)
                throw new StockNotFoundException(1);

            var bagWeightInfoDto = _mapper.Map<BagWeightInfoDto>(bagWeightInfo);
            return bagWeightInfoDto;
        }

        public async Task<BagWeightInfoDto> CreateBagWeightInfoAsync(BagWeightInfoForCreationDto bagWeightInfo)
        {
            // Check if weight already exists
            var existingBagWeightInfo = await _repository.BagWeightInfo.GetBagWeightInfoByWeightAsync(bagWeightInfo.Weight, trackChanges: false);
            if (existingBagWeightInfo != null)
                throw new StockNotFoundException(1);

            var bagWeightInfoEntity = _mapper.Map<BagWeightInfo>(bagWeightInfo);
            _repository.BagWeightInfo.CreateBagWeightInfo(bagWeightInfoEntity);
            await _repository.SaveAsync();

            var bagWeightInfoToReturn = _mapper.Map<BagWeightInfoDto>(bagWeightInfoEntity);
            return bagWeightInfoToReturn;
        }

        public async Task UpdateBagWeightInfoAsync(int bagWeightInfoId, BagWeightInfoForUpdateDto bagWeightInfoForUpdate, bool trackChanges)
        {
            var bagWeightInfo = await GetBagWeightInfoAndCheckIfItExists(bagWeightInfoId, trackChanges);

            // Check if the new weight already exists in another record
            var existingBagWeightInfo = await _repository.BagWeightInfo.GetBagWeightInfoByWeightAsync(bagWeightInfoForUpdate.Weight, trackChanges: false);
            if (existingBagWeightInfo != null && existingBagWeightInfo.Id != bagWeightInfoId)
                throw new StockNotFoundException(1);

            _mapper.Map(bagWeightInfoForUpdate, bagWeightInfo);
            await _repository.SaveAsync();
        }

        public async Task DeleteBagWeightInfoAsync(int bagWeightInfoId, bool trackChanges)
        {
            var bagWeightInfo = await GetBagWeightInfoAndCheckIfItExists(bagWeightInfoId, trackChanges);
            _repository.BagWeightInfo.DeleteBagWeightInfo(bagWeightInfo);
            await _repository.SaveAsync();
        }

        private async Task<BagWeightInfo> GetBagWeightInfoAndCheckIfItExists(int id, bool trackChanges)
        {
            var bagWeightInfo = await _repository.BagWeightInfo.GetBagWeightInfoAsync(id, trackChanges);
            if (bagWeightInfo is null)
                throw new StockNotFoundException(id);
            return bagWeightInfo;
        }
    }
}