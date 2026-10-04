using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class BlogIndexVM
{
    public List<BlogPost> Posts { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public int TotalCount { get; set; } = 0;
    public string? CurrentCategory { get; set; }
    public string? SearchKeyword { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<BlogPost> RecentPosts { get; set; } = new();
}

public class BlogDetailVM
{
    public BlogPost Post { get; set; } = null!;
    public List<BlogPost> RecentPosts { get; set; } = new();
    public BlogPost? PreviousPost { get; set; }
    public BlogPost? NextPost { get; set; }
}
