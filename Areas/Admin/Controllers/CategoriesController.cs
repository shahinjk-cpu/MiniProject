using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Areas.Admin.ViewModels;
using PetShopApp.Data;
using PetShopApp.Models;

namespace PetShopApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .Include(c => c.Products.Where(p => !p.IsDeleted))
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CategoryFormVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormVM model)
    {
        if (ModelState.IsValid)
        {
            string slug = string.IsNullOrWhiteSpace(model.Slug)
                ? model.Name.ToLower().Replace(" ", "-").Replace("&", "and")
                : model.Slug.ToLower().Replace(" ", "-");

            var category = new Category
            {
                Name = model.Name.Trim(),
                Slug = slug,
                Description = model.Description,
                IconClass = model.IconClass,
                ImageUrl = model.ImageUrl,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kateqoriya uğurla əlavə edildi!";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null || category.IsDeleted)
        {
            return NotFound();
        }

        var vm = new CategoryFormVM
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            IconClass = category.IconClass,
            ImageUrl = category.ImageUrl,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormVM model)
    {
        if (ModelState.IsValid)
        {
            var category = await _context.Categories.FindAsync(model.Id);
            if (category == null || category.IsDeleted)
            {
                return NotFound();
            }

            category.Name = model.Name.Trim();
            category.Slug = string.IsNullOrWhiteSpace(model.Slug)
                ? model.Name.ToLower().Replace(" ", "-").Replace("&", "and")
                : model.Slug.ToLower().Replace(" ", "-");
            category.Description = model.Description;
            category.IconClass = model.IconClass;
            category.ImageUrl = model.ImageUrl;
            category.DisplayOrder = model.DisplayOrder;
            category.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kateqoriya uğurla yeniləndi!";
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null || category.IsDeleted)
        {
            return NotFound();
        }

        category.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kateqoriya uğurla silindi!";
        return RedirectToAction(nameof(Index));
    }
}
