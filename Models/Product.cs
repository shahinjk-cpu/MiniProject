namespace PetShopApp.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int StockQuantity { get; set; } = 50;
    public double Rating { get; set; } = 5.0;
    public int ReviewCount { get; set; } = 0;
    public string MainImageUrl { get; set; } = string.Empty;

    public bool IsFeatured { get; set; } = false;
    public bool IsBestSeller { get; set; } = false;
    public bool IsClothing { get; set; } = false;
    public bool IsFoodie { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}
