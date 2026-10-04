using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class HomeIndexVM
{
    public List<SliderItem> Sliders { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Product> ClothingProducts { get; set; } = new();
    public List<Product> FoodieProducts { get; set; } = new();
    public List<Product> BestSellingProducts { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = new();
    public List<BlogPost> LatestBlogs { get; set; } = new();
    public List<InstagramPost> InstagramPosts { get; set; } = new();
}
