using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;

namespace PetShopApp.Controllers.Api;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsApiController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Bütün aktiv məhsulların siyahısını və ya filterlənmiş nəticələri qaytarır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var query = _context.Products
            .Where(p => p.IsActive && !p.IsDeleted)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category != null && p.Category.Slug == category);
        }

        int total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Slug,
                p.Price,
                p.OldPrice,
                p.SKU,
                p.StockQuantity,
                Category = p.Category != null ? p.Category.Name : null,
                p.MainImageUrl,
                p.Rating,
                p.ReviewCount,
                p.IsClothing,
                p.IsFoodie,
                p.IsBestSeller,
                p.IsFeatured
            })
            .ToListAsync();

        return Ok(new
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Products = items
        });
    }

    /// <summary>
    /// İD üzrə məhsulun detallı məlumatlarını qaytarır.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            return NotFound(new { message = "Məhsul tapılmadı." });
        }

        return Ok(new
        {
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.AdditionalInfo,
            product.Price,
            product.OldPrice,
            product.SKU,
            product.StockQuantity,
            Category = product.Category?.Name,
            product.MainImageUrl,
            Gallery = product.Images.Select(i => i.ImageUrl),
            Reviews = product.Reviews.Select(r => new { r.FullName, r.Rating, r.Comment, r.CreatedAt }),
            product.Rating,
            product.ReviewCount
        });
    }
}
