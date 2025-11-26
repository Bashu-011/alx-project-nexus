namespace E_Commerce.Application.DTOs.Categories;

//a request to update an existing category

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
