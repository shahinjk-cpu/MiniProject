using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Extensions;
using PetShopApp.Models;
using PetShopApp.ViewModels;

namespace PetShopApp.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public HeaderViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
        var categories = await _context.Categories
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var model = new HeaderVM
        {
            SiteName = settings.GetValueOrDefault("SiteName", "Waggy"),
            LogoUrl = settings.GetValueOrDefault("LogoUrl", "logo.png"),
            ContactPhone = settings.GetValueOrDefault("ContactPhone", "+1 800 555 0199"),
            ContactEmail = settings.GetValueOrDefault("ContactEmail", "info@waggypetshop.com"),
            Categories = categories,
            CartItems = cartItems,
            CartItemCount = cartItems.Sum(c => c.Quantity),
            CartSubTotal = cartItems.Sum(c => c.TotalPrice)
        };

        return View(model);
    }
}
