namespace E_Commerce.Application.DTOs.Categories;

//creating a category request

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}