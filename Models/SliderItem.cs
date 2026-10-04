namespace PetShopApp.Models;

public class SliderItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string? DiscountText { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ButtonText { get; set; } = "Shop Now";
    public string ButtonUrl { get; set; } = "/Shop";
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
