using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.ViewModels;

namespace PetShopApp.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public FooterViewComponent(AppDbContext context)
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

        var model = new FooterVM
        {
            SiteName = settings.GetValueOrDefault("SiteName", "Waggy"),
            LogoUrl = settings.GetValueOrDefault("LogoUrl", "logo.png"),
            ContactAddress = settings.GetValueOrDefault("ContactAddress", "123 Pet Street, Animal City, AC 45678"),
            ContactPhone = settings.GetValueOrDefault("ContactPhone", "+1 800 555 0199"),
            ContactEmail = settings.GetValueOrDefault("ContactEmail", "info@waggypetshop.com"),
            FacebookUrl = settings.GetValueOrDefault("FacebookUrl", "#"),
            TwitterUrl = settings.GetValueOrDefault("TwitterUrl", "#"),
            InstagramUrl = settings.GetValueOrDefault("InstagramUrl", "#"),
            YouTubeUrl = settings.GetValueOrDefault("YouTubeUrl", "#"),
            Categories = categories
        };

        return View(model);
    }
}
