namespace PetShopApp.Models;

public class BlogComment : BaseEntity
{
    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    public string AuthorName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true;
}
