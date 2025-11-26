namespace E_Commerce.Application.DTOs.Products;

//parameters for querying products with pagination, filtering, and sorting
public class ProductQueryParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public bool IsDescending { get; set; } = true;
}
