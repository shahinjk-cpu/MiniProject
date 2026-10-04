using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class FooterVM
{
    public string SiteName { get; set; } = "Waggy";
    public string LogoUrl { get; set; } = "logo.png";
    public string ContactAddress { get; set; } = "123 Pet Street, Animal City, AC 45678";
    public string ContactPhone { get; set; } = "+1 800 555 0199";
    public string ContactEmail { get; set; } = "info@waggypetshop.com";
    public string FacebookUrl { get; set; } = "#";
    public string TwitterUrl { get; set; } = "#";
    public string InstagramUrl { get; set; } = "#";
    public string YouTubeUrl { get; set; } = "#";
    public List<Category> Categories { get; set; } = new();
}
