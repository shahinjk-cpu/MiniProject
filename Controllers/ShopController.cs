using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Extensions;
using PetShopApp.Models;
using PetShopApp.ViewModels;

namespace PetShopApp.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? category,
        string? search,
        string? sort,
        decimal? minPrice,
        decimal? maxPrice,
        int page = 1)
    {
        const int pageSize = 9;
        if (page < 1) page = 1;

        var query = _context.Products
            .Where(p => p.IsActive && !p.IsDeleted)
            .Include(p => p.Category)
            .AsQueryable();

        // 1. Kateqoriya filtrasiyası
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category.Slug == category);
        }

        // 2. Axtarış
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.Description.ToLower().Contains(searchLower));
        }

        // 3. Qiymət filtri
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // 4. Sıralama
        query = sort switch
        {
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating_desc" => query.OrderByDescending(p => p.Rating),
            _ => query.OrderByDescending(p => p.Id)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page > totalPages) page = totalPages;

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var categories = await _context.Categories
            .Where(c => c.IsActive && !c.IsDeleted)
            .Include(c => c.Products)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var vm = new ShopVM
        {
            Products = products,
            Categories = categories,
            SelectedCategory = category,
            SearchQuery = search,
            SortOrder = sort,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems,
            PageSize = pageSize
        };

        ViewData["Title"] = string.IsNullOrWhiteSpace(category) ? "Shop All Products" : $"Shop - {category}";
        return View(vm);
    }

    [HttpGet]
    [Route("Shop/Detail/{id?}")]
    public async Task<IActionResult> Detail(string? id, string? slug)
    {
        var target = !string.IsNullOrWhiteSpace(slug) ? slug : id;
        if (string.IsNullOrWhiteSpace(target)) return RedirectToAction(nameof(Index));

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Slug == target && p.IsActive && !p.IsDeleted);

        // Fallback: əgər ID ilə müraciət edilibsə
        if (product == null && int.TryParse(target, out int prodId))
        {
            product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == prodId && p.IsActive && !p.IsDeleted);
        }

        if (product == null) return NotFound();

        var relatedProducts = await _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive && !p.IsDeleted)
            .Take(4)
            .ToListAsync();

        if (relatedProducts.Count < 4)
        {
            var moreProducts = await _context.Products
                .Where(p => p.Id != product.Id && !relatedProducts.Select(r => r.Id).Contains(p.Id) && p.IsActive && !p.IsDeleted)
                .Take(4 - relatedProducts.Count)
                .ToListAsync();
            relatedProducts.AddRange(moreProducts);
        }

        var vm = new ProductDetailVM
        {
            Product = product,
            RelatedProducts = relatedProducts
        };

        ViewData["Title"] = product.Name;
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(int productId, string fullName, string email, string comment, int rating = 5)
    {
        var product = await _context.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product != null && !string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(comment))
        {
            var review = new ProductReview
            {
                ProductId = productId,
                FullName = fullName.Trim(),
                Email = email?.Trim() ?? "",
                Comment = comment.Trim(),
                Rating = Math.Clamp(rating, 1, 5),
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            // Orta reytinqi yenilə
            var allReviews = await _context.ProductReviews.Where(r => r.ProductId == productId && r.IsApproved).ToListAsync();
            product.ReviewCount = allReviews.Count;
            product.Rating = allReviews.Any() ? Math.Round(allReviews.Average(r => r.Rating), 1) : 5.0;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your review has been successfully submitted!";
            return RedirectToAction(nameof(Detail), new { id = product.Slug });
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Wishlist()
    {
        ViewData["Title"] = "My Wishlist";
        var wishlistIds = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();

        var products = await _context.Products
            .Where(p => wishlistIds.Contains(p.Id) && p.IsActive && !p.IsDeleted)
            .Include(p => p.Category)
            .ToListAsync();

        return View(products);
    }

    public IActionResult AddToWishlist(int productId, string? returnUrl)
    {
        var wishlist = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();
        if (!wishlist.Contains(productId))
        {
            wishlist.Add(productId);
            HttpContext.Session.SetObjectAsJson("Wishlist", wishlist);
            TempData["SuccessMessage"] = "Product added to your wishlist!";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Wishlist));
    }

    public IActionResult RemoveFromWishlist(int productId)
    {
        var wishlist = HttpContext.Session.GetObjectFromJson<List<int>>("Wishlist") ?? new List<int>();
        if (wishlist.Contains(productId))
        {
            wishlist.Remove(productId);
            HttpContext.Session.SetObjectAsJson("Wishlist", wishlist);
        }
        return RedirectToAction(nameof(Wishlist));
    }
}
