using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Models;
using PetShopApp.ViewModels;

namespace PetShopApp.Controllers;

public class BlogController : Controller
{
    private readonly AppDbContext _context;

    public BlogController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? category, string? search, int page = 1)
    {
        int pageSize = 6;
        var query = _context.BlogPosts
            .Where(b => !b.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(b => b.CategoryName == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search) || 
                                     b.ShortDescription.Contains(search) || 
                                     b.Content.Contains(search));
        }

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var posts = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var categories = await _context.BlogPosts
            .Where(b => !b.IsDeleted)
            .Select(b => b.CategoryName)
            .Distinct()
            .ToListAsync();

        var recentPosts = await _context.BlogPosts
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Take(3)
            .ToListAsync();

        var viewModel = new BlogIndexVM
        {
            Posts = posts,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalCount = totalCount,
            CurrentCategory = category,
            SearchKeyword = search,
            Categories = categories,
            RecentPosts = recentPosts
        };

        return View(viewModel);
    }

    [Route("Blog/Detail/{id?}")]
    public async Task<IActionResult> Detail(string? id, string? slug)
    {
        BlogPost? post = null;

        if (!string.IsNullOrEmpty(slug))
        {
            post = await _context.BlogPosts
                .Include(b => b.Comments.Where(c => c.IsApproved))
                .FirstOrDefaultAsync(b => b.Slug == slug && !b.IsDeleted);
        }
        else if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int parsedId))
        {
            post = await _context.BlogPosts
                .Include(b => b.Comments.Where(c => c.IsApproved))
                .FirstOrDefaultAsync(b => b.Id == parsedId && !b.IsDeleted);
        }

        if (post == null)
        {
            return NotFound();
        }

        post.ViewCount++;
        await _context.SaveChangesAsync();

        var previousPost = await _context.BlogPosts
            .Where(b => !b.IsDeleted && b.Id < post.Id)
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();

        var nextPost = await _context.BlogPosts
            .Where(b => !b.IsDeleted && b.Id > post.Id)
            .OrderBy(b => b.Id)
            .FirstOrDefaultAsync();

        var recentPosts = await _context.BlogPosts
            .Where(b => !b.IsDeleted && b.Id != post.Id)
            .OrderByDescending(b => b.CreatedAt)
            .Take(3)
            .ToListAsync();

        var viewModel = new BlogDetailVM
        {
            Post = post,
            PreviousPost = previousPost,
            NextPost = nextPost,
            RecentPosts = recentPosts
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int blogPostId, string authorName, string email, string? website, string content)
    {
        if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(content))
        {
            TempData["CommentError"] = "Zəhmət olmasa bütün tələb olunan xanaları doldurun.";
            return RedirectToAction(nameof(Detail), new { id = blogPostId });
        }

        var post = await _context.BlogPosts.FindAsync(blogPostId);
        if (post == null)
        {
            return NotFound();
        }

        var comment = new BlogComment
        {
            BlogPostId = blogPostId,
            AuthorName = authorName.Trim(),
            Email = email.Trim(),
            Website = website?.Trim(),
            Content = content.Trim(),
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogComments.Add(comment);
        await _context.SaveChangesAsync();

        TempData["CommentSuccess"] = "Rəyiniz uğurla əlavə edildi!";
        return RedirectToAction(nameof(Detail), new { id = blogPostId });
    }
}
