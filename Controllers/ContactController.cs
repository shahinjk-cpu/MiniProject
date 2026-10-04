using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Models;

namespace PetShopApp.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _context;

    public ContactController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
        ViewBag.Address = settings.GetValueOrDefault("Address", "730 Glenstone Ave 65802, Springfield, US");
        ViewBag.Phone = settings.GetValueOrDefault("Phone", "+123 987 321");
        ViewBag.Email = settings.GetValueOrDefault("Email", "contact@waggypetshop.com");

        return View(new ContactMessage());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactMessage model)
    {
        if (ModelState.IsValid)
        {
            model.CreatedAt = DateTime.UtcNow;
            model.IsRead = false;

            _context.ContactMessages.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mesajınız qəbul edildi! Ən qısa zamanda sizinlə əlaqə saxlayacağıq.";
            return RedirectToAction(nameof(Index));
        }

        var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
        ViewBag.Address = settings.GetValueOrDefault("Address", "730 Glenstone Ave 65802, Springfield, US");
        ViewBag.Phone = settings.GetValueOrDefault("Phone", "+123 987 321");
        ViewBag.Email = settings.GetValueOrDefault("Email", "contact@waggypetshop.com");

        return View(model);
    }
}
