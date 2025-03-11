using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Product;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ProductController(IServiceManager service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _service.ProductService.GetAllProductsAsync(trackChanges: true);
            return Ok(products);
        }

        [HttpGet("{tagID}", Name = "tagID")]
        public async Task<IActionResult> GetProduct(string tagID)
        {
            var product = await _service.ProductService.GetProductByTagIDAsync(tagID, trackChanges: true);
            return Ok(product);
        }
        [HttpGet("{id:long}", Name = "ProductById")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var product = await _service.ProductService.GetProductAsync(id, trackChanges: true);
            return Ok(product);
        }

        [HttpGet("distributor/{distributorId:long}")]
        public async Task<IActionResult> GetProductsByDistributor(long distributorId)
        {
            var products = await _service.ProductService.GetProductsByDistributorAsync(distributorId, trackChanges: true);
            return Ok(products);
        }

        [HttpGet("productInformation/{productInformationId:long}")]
        public async Task<IActionResult> GetProductsByProductInformation(long productInformationId)
        {
            var products = await _service.ProductService.GetProductsByProductInformationAsync(productInformationId, trackChanges: true);
            return Ok(products);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateProduct([FromBody] ProductForCreationDto product)
        {
            var createdProduct = await _service.ProductService.CreateProductAsync(product);
            return CreatedAtRoute("ProductById", new { id = createdProduct.Id }, createdProduct);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            await _service.ProductService.DeleteProductAsync(id, trackChanges: true);
            return NoContent();
        }

        [HttpPut("{id:long}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] ProductForUpdateDto product)
        {
            await _service.ProductService.UpdateProductAsync(id, product, trackChanges: true);
            return NoContent();
        }
    }
}