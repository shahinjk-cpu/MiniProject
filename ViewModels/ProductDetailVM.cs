using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class ProductDetailVM
{
    public Product Product { get; set; } = null!;
    public List<Product> RelatedProducts { get; set; } = new();

    // Review Form Binding
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
}
