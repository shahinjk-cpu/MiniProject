using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class HeaderVM
{
    public string SiteName { get; set; } = "Waggy";
    public string LogoUrl { get; set; } = "logo.png";
    public string ContactPhone { get; set; } = "+1 800 555 0199";
    public string ContactEmail { get; set; } = "info@waggypetshop.com";
    public List<Category> Categories { get; set; } = new();
    public int CartItemCount { get; set; } = 0;
    public decimal CartSubTotal { get; set; } = 0m;
    public List<CartItem> CartItems { get; set; } = new();
}
