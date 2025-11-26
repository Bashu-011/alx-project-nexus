using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.DTOs.Products;

namespace E_Commerce.Application.Interfaces;

//interface for product services - contains b/s logic for managing products
public interface IProductService
{
    Task<PagedResponse<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(Guid id);
}
