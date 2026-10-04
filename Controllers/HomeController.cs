using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Models;
using PetShopApp.ViewModels;

namespace PetShopApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sliders = await _context.SliderItems
            .Where(s => s.IsActive && !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();

        var categories = await _context.Categories
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var clothing = await _context.Products
            .Where(p => p.IsActive && !p.IsDeleted && p.IsClothing)
            .Include(p => p.Category)
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();

        var foodies = await _context.Products
            .Where(p => p.IsActive && !p.IsDeleted && p.IsFoodie)
            .Include(p => p.Category)
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();

        var bestSellers = await _context.Products
            .Where(p => p.IsActive && !p.IsDeleted && (p.IsBestSeller || p.IsFeatured))
            .Include(p => p.Category)
            .OrderByDescending(p => p.Rating)
            .Take(8)
            .ToListAsync();

        var testimonials = await _context.Testimonials
            .Where(t => t.IsActive && !t.IsDeleted)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync();

        var blogs = await _context.BlogPosts
            .Where(b => !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .Take(3)
            .ToListAsync();

        var instaPosts = await _context.InstagramPosts
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.DisplayOrder)
            .Take(6)
            .ToListAsync();

        var viewModel = new HomeIndexVM
        {
            Sliders = sliders,
            Categories = categories,
            ClothingProducts = clothing,
            FoodieProducts = foodies,
            BestSellingProducts = bestSellers,
            Testimonials = testimonials,
            LatestBlogs = blogs,
            InstagramPosts = instaPosts
        };

        return View(viewModel);
    }

    public async Task<IActionResult> About()
    {
        ViewData["Title"] = "About Us";
        return View();
    }

    public async Task<IActionResult> Faqs()
    {
        ViewData["Title"] = "Frequently Asked Questions";
        var faqs = await _context.FaqItems
            .Where(f => f.IsActive && !f.IsDeleted)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();

        return View(faqs);
    }

    [HttpPost]
    public async Task<IActionResult> SubscribeNewsletter(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var exists = await _context.NewsletterSubscribers.AnyAsync(n => n.Email == email);
            if (!exists)
            {
                _context.NewsletterSubscribers.Add(new NewsletterSubscriber { Email = email });
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Thank you for subscribing to our newsletter!";
        }

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
