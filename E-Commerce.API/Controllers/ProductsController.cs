using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with filtering, sorting, and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<ProductDto>>>> GetProducts([FromQuery] ProductQueryParameters parameters)
    {
        try
        {
            var result = await _productService.GetProductsAsync(parameters);
            return Ok(ApiResponse<PagedResponse<ProductDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, ApiResponse<PagedResponse<ProductDto>>.ErrorResponse("An error occurred while retrieving products"));
        }
    }

    /// <summary>
    /// Get a product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found"));
            }
            return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while retrieving the product"));
        }
    }

    /// <summary>
    /// Create a new product (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                ApiResponse<ProductDto>.SuccessResponse(product, "Product created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create product");
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while creating the product"));
        }
    }

    /// <summary>
    /// Update an existing product (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update product {ProductId}", id);
            return NotFound(ApiResponse<ProductDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while updating the product"));
        }
    }

    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Product not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the product"));
        }
    }
}