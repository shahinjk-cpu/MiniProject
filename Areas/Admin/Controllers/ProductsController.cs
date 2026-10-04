using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Areas.Admin.ViewModels;
using PetShopApp.Data;
using PetShopApp.Models;

namespace PetShopApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1)
    {
        int pageSize = 10;
        var query = _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var products = await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new ProductFormVM
        {
            CategoryOptions = await GetCategoryOptionsAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormVM model)
    {
        if (ModelState.IsValid)
        {
            string slug = model.Name.ToLower().Replace(" ", "-").Replace("&", "and") + "-" + Guid.NewGuid().ToString().Substring(0, 5);

            var product = new Product
            {
                Name = model.Name.Trim(),
                Slug = slug,
                Description = model.Description ?? string.Empty,
                AdditionalInfo = model.AdditionalInfo ?? string.Empty,
                Price = model.Price,
                OldPrice = model.OldPrice,
                SKU = model.SKU.Trim().ToUpper(),
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                MainImageUrl = string.IsNullOrWhiteSpace(model.MainImageUrl) ? "images/item1.jpg" : model.MainImageUrl.Trim(),
                IsClothing = model.IsClothing,
                IsFoodie = model.IsFoodie,
                IsBestSeller = model.IsBestSeller,
                IsFeatured = model.IsFeatured,
                IsActive = model.IsActive,
                Rating = 5,
                ReviewCount = 1,
                CreatedAt = DateTime.UtcNow
            };

            // Add main image to ProductImages collection as well
            product.Images.Add(new ProductImage
            {
                ImageUrl = product.MainImageUrl,
                IsMain = true,
                DisplayOrder = 1
            });

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Məhsul uğurla əlavə edildi!";
            return RedirectToAction(nameof(Index));
        }

        model.CategoryOptions = await GetCategoryOptionsAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
        {
            return NotFound();
        }

        var vm = new ProductFormVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            AdditionalInfo = product.AdditionalInfo,
            Price = product.Price,
            OldPrice = product.OldPrice,
            SKU = product.SKU,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            MainImageUrl = product.MainImageUrl,
            IsClothing = product.IsClothing,
            IsFoodie = product.IsFoodie,
            IsBestSeller = product.IsBestSeller,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive,
            CategoryOptions = await GetCategoryOptionsAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductFormVM model)
    {
        if (ModelState.IsValid)
        {
            var product = await _context.Products.FindAsync(model.Id);
            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            product.Name = model.Name.Trim();
            product.Description = model.Description ?? string.Empty;
            product.AdditionalInfo = model.AdditionalInfo ?? string.Empty;
            product.Price = model.Price;
            product.OldPrice = model.OldPrice;
            product.SKU = model.SKU.Trim().ToUpper();
            product.StockQuantity = model.StockQuantity;
            product.CategoryId = model.CategoryId;
            product.MainImageUrl = string.IsNullOrWhiteSpace(model.MainImageUrl) ? "images/item1.jpg" : model.MainImageUrl.Trim();
            product.IsClothing = model.IsClothing;
            product.IsFoodie = model.IsFoodie;
            product.IsBestSeller = model.IsBestSeller;
            product.IsFeatured = model.IsFeatured;
            product.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Məhsul uğurla yeniləndi!";
            return RedirectToAction(nameof(Index));
        }

        model.CategoryOptions = await GetCategoryOptionsAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.IsDeleted)
        {
            return NotFound();
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Məhsul uğurla silindi!";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
    {
        return await _context.Categories
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();
    }
}
