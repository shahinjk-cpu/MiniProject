using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;

namespace PetShopApp.Controllers.Api;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoriesApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesApiController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Bütün aktiv kateqoriyaları və məhsul saylarını qaytarır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.IconClass,
                c.ImageUrl,
                ProductsCount = c.Products.Count(p => p.IsActive && !p.IsDeleted)
            })
            .ToListAsync();

        return Ok(categories);
    }
}
