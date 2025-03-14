using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickStart.Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects.Product;

namespace QuickStart.Presentation.Controllers
{
    [Route("api/products")]
    [ApiController]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ProductController(IServiceManager service) => _service = service;

        [HttpGet]
        [AuthorizePermission("Products", "View")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.ProductService.GetAllProductsAsync(trackChanges: false);
            return Ok(products);
        }

        [HttpGet("{productId:int}", Name = "GetProductById")]
        [AuthorizePermission("Products", "View")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var product = await _service.ProductService.GetProductAsync(productId, trackChanges: false);
            return Ok(product);
        }

        [HttpGet("tag/{tagId}")]
        [AuthorizePermission("Products", "View")]
        public async Task<IActionResult> GetProductByTagID(string tagId)
        {
            var product = await _service.ProductService.GetProductByTagIDAsync(tagId, trackChanges: false);
            return Ok(product);
        }

        [HttpGet("by-distributor/{distributorId:int}")]
        [AuthorizePermission("Products", "View")]
        public async Task<IActionResult> GetProductsByDistributor(int distributorId)
        {
            var products = await _service.ProductService.GetProductsByDistributorAsync(distributorId, trackChanges: false);
            return Ok(products);
        }

        [HttpGet("by-order-detail/{orderDetailId:int}")]
        [AuthorizePermission("Products", "View")]
        public async Task<IActionResult> GetProductsByOrderDetail(int orderDetailId)
        {
            var products = await _service.ProductService.GetProductsByOrderDetailAsync(orderDetailId, trackChanges: false);
            return Ok(products);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Products", "Create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductForCreationDto product)
        {
            var createdProduct = await _service.ProductService.CreateProductAsync(product);
            return CreatedAtRoute("GetProductById", new { productId = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{productId:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AuthorizePermission("Products", "Update")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductForUpdateDto productForUpdate)
        {
            await _service.ProductService.UpdateProductAsync(productId, productForUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{productId:int}")]
        [AuthorizePermission("Products", "Delete")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _service.ProductService.DeleteProductAsync(productId, trackChanges: false);
            return NoContent();
        }
    }
}