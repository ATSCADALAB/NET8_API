using AutoMapper;
using Contracts;
using Entities.Exceptions.Distributor;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Distributor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class DistributorService : IDistributorService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public DistributorService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync(bool trackChanges)
        {
            var distributors = await _repository.Distributor.GetAllDistributorsAsync(trackChanges);
            var distributorsDto = _mapper.Map<IEnumerable<DistributorDto>>(distributors);
            return distributorsDto;
        }

        public async Task<DistributorDto> GetDistributorAsync(int distributorId, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);
            var distributorDto = _mapper.Map<DistributorDto>(distributor);
            return distributorDto;
        }

        public async Task<DistributorDto> GetDistributorByCodeAsync(string distributorCode, bool trackChanges)
        {
            var distributor = await _repository.Distributor.GetDistributorByCodeAsync(distributorCode, trackChanges);
            if (distributor is null)
                throw new DistributorNotFoundException(0);

            var distributorDto = _mapper.Map<DistributorDto>(distributor);
            return distributorDto;
        }

        public async Task<IEnumerable<DistributorDto>> GetDistributorsByAreaAsync(int areaId, bool trackChanges)
        {
            var distributors = await _repository.Distributor.GetDistributorsByAreaAsync(areaId, trackChanges);
            var distributorsDto = _mapper.Map<IEnumerable<DistributorDto>>(distributors);
            return distributorsDto;
        }

        public async Task<DistributorDto> CreateDistributorAsync(DistributorForCreationDto distributor)
        {
            var check = await GetDistributorAndCheckIfItExists(distributor.DistributorCode, false);
            if (distributor == null)
                throw new ArgumentNullException(nameof(distributor), "DistributorForCreationDto cannot be null.");
            if(check == null)
            {
                var distributorEntity = _mapper.Map<Distributor>(distributor);
                _repository.Distributor.CreateDistributor(distributorEntity);
                await _repository.SaveAsync();

                var distributorToReturn = _mapper.Map<DistributorDto>(distributorEntity);
                return distributorToReturn;
            }
            return null;
        }

        public async Task UpdateDistributorAsync(int distributorId, DistributorForUpdateDto distributorForUpdate, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);
            _mapper.Map(distributorForUpdate, distributor);
            await _repository.SaveAsync();
        }

        public async Task DeleteDistributorAsync(int distributorId, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);
            _repository.Distributor.DeleteDistributor(distributor);
            await _repository.SaveAsync();
        }
        private async Task<Distributor> GetDistributorAndCheckIfItExists(string code, bool trackChanges)
        {
            var distributor = await _repository.Distributor.GetDistributorByCodeAsync(code, trackChanges);
            if (distributor != null)
                throw new DistributorNotFoundException(0);
            return distributor;
        }
        private async Task<Distributor> GetDistributorAndCheckIfItExists(int id, bool trackChanges)
        {
            var distributor = await _repository.Distributor.GetDistributorByIdAsync(id, trackChanges);
            if (distributor is null)
                throw new DistributorNotFoundException(id);
            return distributor;
        }
        
    }
}