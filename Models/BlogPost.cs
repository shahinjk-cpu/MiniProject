namespace PetShopApp.Models;

public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "Admin";
    public string CategoryName { get; set; } = "Pets";
    public int ViewCount { get; set; } = 0;

    public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();
}
