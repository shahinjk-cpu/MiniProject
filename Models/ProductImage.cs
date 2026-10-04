namespace PetShopApp.Models;

public class ProductImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
