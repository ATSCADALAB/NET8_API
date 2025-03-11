using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Exceptions.Product;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Product;

namespace Service
{
    internal sealed class ProductService : IProductService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public ProductService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateProductAsync(ProductForCreationDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "ProductForCreationDto cannot be null.");

            var productEntity = _mapper.Map<Product>(product);

            _repository.Product.CreateProduct(productEntity);
            await _repository.SaveAsync();

            var productToReturn = _mapper.Map<ProductDto>(productEntity);
            return productToReturn;
        }

        public async Task DeleteProductAsync(long productId, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);

            _repository.Product.DeleteProduct(product);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool trackChanges)
        {
            var products = await _repository.Product.GetProductsAsync(trackChanges);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);

            return productsDto;
        }

        public async Task<ProductDto> GetProductAsync(long productId, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }
        public async Task<ProductDto> GetProductByTagIDAsync(string productId, bool trackChanges)
        {
            var product = await GetProductByNameAndCheckIfItExists(productId, trackChanges);

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByDistributorAsync(long distributorId, bool trackChanges)
        {
            var products = await _repository.Product.GetProductsByDistributorAsync(distributorId, trackChanges);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productsDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByProductInformationAsync(long productInformationId, bool trackChanges)
        {
            var products = await _repository.Product.GetProductsByProductInformationAsync(productInformationId, trackChanges);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productsDto;
        }

        public async Task UpdateProductAsync(long productId, ProductForUpdateDto productForUpdate, bool trackChanges)
        {
            var product = await GetProductAndCheckIfItExists(productId, trackChanges);

            _mapper.Map(productForUpdate, product);
            await _repository.SaveAsync();
        }

        private async Task<Product> GetProductAndCheckIfItExists(long id, bool trackChanges)
        {
            var product = await _repository.Product.GetProductAsync(id, trackChanges);
            if (product is null)
                throw new ProductNotFoundException(id);

            return product;
        }
        private async Task<Product> GetProductByNameAndCheckIfItExists(string id, bool trackChanges)
        {
            var product = await _repository.Product.GetProductByTagIDAsync(id, trackChanges);
            if (product is null)
                throw new ProductNotFoundException(1);

            return product;
        }
    }
}