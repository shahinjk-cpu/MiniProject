namespace PetShopApp.Models;

public class InstagramPost : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public string PostUrl { get; set; } = "#";
    public int DisplayOrder { get; set; } = 0;
}
