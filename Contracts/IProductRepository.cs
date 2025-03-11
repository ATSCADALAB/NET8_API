using Entities.Models;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(bool trackChanges);
    Task<Product> GetProductAsync(long productId, bool trackChanges);
    Task<Product> GetProductByTagIDAsync(string tagID, bool trackChanges);
    Task<IEnumerable<Product>> GetProductsByDistributorAsync(long distributorId, bool trackChanges);
    Task<IEnumerable<Product>> GetProductsByProductInformationAsync(long productInformationId, bool trackChanges);
    void CreateProduct(Product product);
    void DeleteProduct(Product product);
}