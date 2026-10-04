using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;

namespace PetShopApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class MessagesController : Controller
{
    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(bool? unreadOnly, int page = 1)
    {
        int pageSize = 10;
        var query = _context.ContactMessages
            .Where(m => !m.IsDeleted)
            .AsQueryable();

        if (unreadOnly == true)
        {
            query = query.Where(m => !m.IsRead);
        }

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Clamp(page, 1, Math.Max(1, totalPages));

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.UnreadOnly = unreadOnly;

        return View(messages);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null || message.IsDeleted)
        {
            return NotFound();
        }

        if (!message.IsRead)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null || message.IsDeleted)
        {
            return NotFound();
        }

        message.IsDeleted = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Mesaj silindi.";
        return RedirectToAction(nameof(Index));
    }
}
