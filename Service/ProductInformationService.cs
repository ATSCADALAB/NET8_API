using AutoMapper;
using Contracts;
using Entities.Exceptions.ProductInformation;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.ProductInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<ProductInformationDto>> GetAllProductInformationsAsync(bool trackChanges)
        {
            var productInformations = await _repository.ProductInformation.GetAllProductInformationsAsync(trackChanges);
            var productInformationsDto = _mapper.Map<IEnumerable<ProductInformationDto>>(productInformations);
            return productInformationsDto;
        }

        public async Task<ProductInformationDto> GetProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);
            var productInformationDto = _mapper.Map<ProductInformationDto>(productInformation);
            return productInformationDto;
        }

        public async Task<ProductInformationDto> GetProductInformationByCodeAsync(string productCode, bool trackChanges)
        {
            var productInformation = await _repository.ProductInformation.GetProductInformationByCodeAsync(productCode, trackChanges);
            if (productInformation is null)
                throw new ProductInformationNotFoundException(0);

            var productInformationDto = _mapper.Map<ProductInformationDto>(productInformation);
            return productInformationDto;
        }

        public async Task<ProductInformationDto> CreateProductInformationAsync(ProductInformationForCreationDto productInformation)
        {
            if (productInformation == null)
                throw new ArgumentNullException(nameof(productInformation), "ProductInformationForCreationDto cannot be null.");

            var productInformationEntity = _mapper.Map<ProductInformation>(productInformation);
            _repository.ProductInformation.CreateProductInformation(productInformationEntity);
            await _repository.SaveAsync();

            var productInformationToReturn = _mapper.Map<ProductInformationDto>(productInformationEntity);
            return productInformationToReturn;
        }

        public async Task UpdateProductInformationAsync(int productInformationId, ProductInformationForUpdateDto productInformationForUpdate, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);
            _mapper.Map(productInformationForUpdate, productInformation);
            await _repository.SaveAsync();
        }

        public async Task DeleteProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var productInformation = await GetProductInformationAndCheckIfItExists(productInformationId, trackChanges);
            _repository.ProductInformation.DeleteProductInformation(productInformation);
            await _repository.SaveAsync();
        }

        private async Task<ProductInformation> GetProductInformationAndCheckIfItExists(int id, bool trackChanges)
        {
            var productInformation = await _repository.ProductInformation.GetProductInformationByIdAsync(id, trackChanges);
            if (productInformation is null)
                throw new ProductInformationNotFoundException(id);
            return productInformation;
        }
    }
}