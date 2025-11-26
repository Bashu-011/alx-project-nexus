using AutoMapper;
using E_Commerce.Application.DTOs.Categories;
using E_Commerce.Application.Interfaces;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;
using ECommerce.Core.Interfaces;

namespace E_Commerce.Application.Services;

// Service for managing category CrUD operations
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    //constructor with dependency injection
    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    //method to get all categories
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    //method to get category by id
    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    //method to create a new category
    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        category.Slug = GenerateSlug(request.Name);

        var createdCategory = await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    //method to update an existing category
    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        //fetch existing category
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            throw new InvalidOperationException("Category not found");
        }

        _mapper.Map(request, existingCategory);
        existingCategory.Slug = GenerateSlug(request.Name);

        await _categoryRepository.UpdateAsync(existingCategory);

        var updatedCategory = await _categoryRepository.GetByIdAsync(id);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }

    //method to delete a category
    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        //check if category has products
        if (category.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete category with existing products");
        }

        await _categoryRepository.DeleteAsync(category);
        return true;
    }

    //helper method to generate slug from category name
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