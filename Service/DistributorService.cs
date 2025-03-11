using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Exceptions.Distributor;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Distributor;

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

        public async Task<DistributorDto> CreateDistributorAsync(DistributorForCreationDto distributor)
        {
            var distributorEntity = _mapper.Map<Distributor>(distributor);

            _repository.Distributor.CreateDistributor(distributorEntity);
            await _repository.SaveAsync();

            var distributorToReturn = _mapper.Map<DistributorDto>(distributorEntity);
            return distributorToReturn;
        }

        public async Task DeleteDistributorAsync(long distributorId, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);

            _repository.Distributor.DeleteDistributor(distributor);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync(bool trackChanges)
        {
            var distributors = await _repository.Distributor.GetDistributorsAsync(trackChanges);

            var distributorsDto = _mapper.Map<IEnumerable<DistributorDto>>(distributors);

            return distributorsDto;
        }
        public async Task<IEnumerable<DistributorDto>> GetDistributorsByNameAsync(string Name)
        {
            var distributors = await _repository.Distributor.GetDistributorsByNameAsync(Name);

            var distributorsDto = _mapper.Map<IEnumerable<DistributorDto>>(distributors);

            return distributorsDto;
        }

        public async Task<DistributorDto> GetDistributorAsync(long distributorId, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);

            var distributorDto = _mapper.Map<DistributorDto>(distributor);
            return distributorDto;
        }

        public async Task UpdateDistributorAsync(long distributorId, DistributorForUpdateDto distributorForUpdate, bool trackChanges)
        {
            var distributor = await GetDistributorAndCheckIfItExists(distributorId, trackChanges);

            _mapper.Map(distributorForUpdate, distributor);
            await _repository.SaveAsync();
        }

        private async Task<Distributor> GetDistributorAndCheckIfItExists(long id, bool trackChanges)
        {
            var distributor = await _repository.Distributor.GetDistributorAsync(id, trackChanges);
            if (distributor is null)
                throw new DistributorNotFoundException(id);

            return distributor;
        }
        public async Task CreateDistributorsBatchAsync(IEnumerable<DistributorForCreationDto> distributors)
        {
            var entities = distributors.Select(d => new Distributor
            {
                DistributorCode = d.DistributorCode,
                DistributorName = d.DistributorName,
                Address = d.Address,
                PhoneNumber = d.PhoneNumber,
                ContactSource = d.ContactSource,
                Area = d.Area,
                Note = d.Note,
                IsActive = true
            }).ToList();

            await _repository.Distributor.AddRangeAsync(entities);
            await _repository.SaveAsync();
        }
    }
}