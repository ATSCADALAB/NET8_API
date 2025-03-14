using AutoMapper;
using Contracts;
using Entities.Exceptions.Area;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Area;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class AreaService : IAreaService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public AreaService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AreaDto>> GetAllAreasAsync(bool trackChanges)
        {
            var areas = await _repository.Area.GetAllAreasAsync(trackChanges);
            var areasDto = _mapper.Map<IEnumerable<AreaDto>>(areas);
            return areasDto;
        }

        public async Task<AreaDto> GetAreaAsync(int areaId, bool trackChanges)
        {
            var area = await GetAreaAndCheckIfItExists(areaId, trackChanges);
            var areaDto = _mapper.Map<AreaDto>(area);
            return areaDto;
        }

        public async Task<AreaDto> GetAreaByCodeAsync(string areaCode, bool trackChanges)
        {
            var area = await _repository.Area.GetAreaByCodeAsync(areaCode, trackChanges);
            if (area is null)
                throw new AreaNotFoundException(0); // ID không quan trọng vì dùng Code để tìm

            var areaDto = _mapper.Map<AreaDto>(area);
            return areaDto;
        }

        public async Task<AreaDto> CreateAreaAsync(AreaForCreationDto area)
        {
            if (area == null)
                throw new ArgumentNullException(nameof(area), "AreaForCreationDto cannot be null.");

            var areaEntity = _mapper.Map<Area>(area);
            _repository.Area.CreateArea(areaEntity);
            await _repository.SaveAsync();

            var areaToReturn = _mapper.Map<AreaDto>(areaEntity);
            return areaToReturn;
        }

        public async Task UpdateAreaAsync(int areaId, AreaForUpdateDto areaForUpdate, bool trackChanges)
        {
            var area = await GetAreaAndCheckIfItExists(areaId, trackChanges);
            _mapper.Map(areaForUpdate, area);
            await _repository.SaveAsync();
        }

        public async Task DeleteAreaAsync(int areaId, bool trackChanges)
        {
            var area = await GetAreaAndCheckIfItExists(areaId, trackChanges);
            _repository.Area.DeleteArea(area);
            await _repository.SaveAsync();
        }

        private async Task<Area> GetAreaAndCheckIfItExists(int id, bool trackChanges)
        {
            var area = await _repository.Area.GetAreaByIdAsync(id, trackChanges);
            if (area is null)
                throw new AreaNotFoundException(id);
            return area;
        }
    }
}