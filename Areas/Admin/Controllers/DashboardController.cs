using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Areas.Admin.ViewModels;
using PetShopApp.Data;
using PetShopApp.Models.Enums;

namespace PetShopApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardVM
        {
            TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted),
            TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m,
            PendingOrdersCount = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
            UnreadMessagesCount = await _context.ContactMessages.CountAsync(m => !m.IsRead),
            RecentOrders = await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync(),
            LowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .Take(5)
                .ToListAsync()
        };

        return View(vm);
    }
}
