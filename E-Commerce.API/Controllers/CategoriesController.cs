using E_Commerce.Application.DTOs.Categories;
using E_Commerce.Application.DTOs.Common;
using E_Commerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

//controller to manage category CrUD operations

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    //constructor with dependency injection

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

   

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    
    //Get all categories
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, ApiResponse<IEnumerable<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories"));
        }
    }

    /// <summary>
    /// Get a category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]

    //get category by ID
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found"));
            }
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {CategoryId}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category"));
        }
    }

    /// <summary>
    /// Create a new category (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]

    //create new category
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                ApiResponse<CategoryDto>.SuccessResponse(category, "Category created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while creating the category"));
        }
    }

    /// <summary>
    /// Update an existing category (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]

    //update category
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, request);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update category {CategoryId}", id);
            return NotFound(ApiResponse<CategoryDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while updating the category"));
        }
    }

    /// <summary>
    /// Delete a category (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]

    //delete category
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Category not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Category deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete category {CategoryId}", id);
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the category"));
        }
    }
}