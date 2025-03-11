using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Exceptions.ProductInformation;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.ProductInformation;

namespace Service
{
    internal sealed class ProductInformationService : IProductInformationService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public ProductInformationService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ProductInformationDto> CreateProductInformationAsync(ProductInformationForCreationDto productInformation)
        {
            var productInformationEntity = _mapper.Map<ProductInformation>(productInformation);

            _repository.ProductInformation.CreateProductInformation(productInformationEntity);
            await _repository.SaveAsync();

            var productInformationToReturn = _mapper.Map<ProductInformationDto>(productInformationEntity);
            return productInformationToReturn;
        }

        public async Task DeleteProductInformationAsync(long productInformationId, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);

            _repository.ProductInformation.DeleteProductInformation(productInformation);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<ProductInformationDto>> GetAllProductInformationsAsync(bool trackChanges)
        {
            var productInformations = await _repository.ProductInformation.GetProductInformationsAsync(trackChanges);

            var productInformationsDto = _mapper.Map<IEnumerable<ProductInformationDto>>(productInformations);

            return productInformationsDto;
        }
        public async Task<IEnumerable<ProductInformationDto>> GetDistributorsByNameAsync(string name)
        {
            var productInformations = await _repository.ProductInformation.GetDistributorsByNameAsync(name);

            var productInformationsDto = _mapper.Map<IEnumerable<ProductInformationDto>>(productInformations);

            return productInformationsDto;
        }

        public async Task<ProductInformationDto> GetProductInformationAsync(long productInformationId, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);

            var productInformationDto = _mapper.Map<ProductInformationDto>(productInformation);
            return productInformationDto;
        }

        public async Task UpdateProductInformationAsync(long productInformationId, ProductInformationForUpdateDto productInformationForUpdate, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);

            _mapper.Map(productInformationForUpdate, productInformation);
            await _repository.SaveAsync();
        }

        private async Task<ProductInformation> GetProductInformationAndCheckIfItExists(long id, bool trackChanges)
        {
            var productInformation = await _repository.ProductInformation.GetProductInformationAsync(id, trackChanges);
            if (productInformation is null)
                throw new ProductInformationNotFoundException(id);

            return productInformation;
        }

    }
}