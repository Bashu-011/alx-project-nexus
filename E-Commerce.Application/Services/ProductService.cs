using AutoMapper;
using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Interfaces;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using ECommerce.Core.Interfaces;

namespace E_Commerce.Application.Services;

// Service for managing products CrUD operations

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    //constructor with dependency injection
    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    //method to get paged products with filtering and sorting
    public async Task<PagedResponse<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(
            parameters.PageNumber,
            parameters.PageSize,
            parameters.CategoryId,
            parameters.MinPrice,
            parameters.MaxPrice,
            parameters.SearchTerm,
            parameters.SortBy,
            parameters.IsDescending
        );

        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(items);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        return new PagedResponse<ProductDto>
        {
            Items = productDtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    //method to get product by id

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    //method to create a new product
    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        //verify if category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        var product = _mapper.Map<Product>(request);
        product.Slug = GenerateSlug(request.Name);
        product.IsActive = true;

        var createdProduct = await _productRepository.AddAsync(product);

        //reload with category for mapping
        var productWithCategory = await _productRepository.GetByIdAsync(createdProduct.Id);
        return _mapper.Map<ProductDto>(productWithCategory);
    }

    //method to update an existing product
    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        //verify category exists
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        _mapper.Map(request, existingProduct);
        existingProduct.Slug = GenerateSlug(request.Name);

        await _productRepository.UpdateAsync(existingProduct);

        //reload with category for mapping
        var updatedProduct = await _productRepository.GetByIdAsync(id);
        return _mapper.Map<ProductDto>(updatedProduct);
    }

    //method to delete a product
    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);
        return true;
    }

    //helper method to generate slug from product name
    private string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "");
    }
}