using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Extensions;
using PetShopApp.Models;

namespace PetShopApp.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Shopping Cart";
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        return View(cart);
    }

    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
    {
        if (quantity < 1) quantity = 1;

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive && !p.IsDeleted);
        if (product == null)
        {
            if (IsAjaxRequest()) return Json(new { success = false, message = "Product not found" });
            return NotFound();
        }

        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.MainImageUrl
            });
        }

        HttpContext.Session.SetObjectAsJson("Cart", cart);
        TempData["SuccessMessage"] = $"{product.Name} added to your cart!";

        if (IsAjaxRequest())
        {
            return Json(new
            {
                success = true,
                count = cart.Sum(c => c.Quantity),
                subTotal = cart.Sum(c => c.TotalPrice)
            });
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        if (IsAjaxRequest())
        {
            return Json(new
            {
                success = true,
                count = cart.Sum(c => c.Quantity),
                subTotal = cart.Sum(c => c.TotalPrice),
                itemTotal = item?.TotalPrice ?? 0
            });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [HttpGet]
    public IActionResult RemoveItem(int productId)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);

        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["SuccessMessage"] = "Item removed from cart.";
        }

        if (IsAjaxRequest())
        {
            return Json(new
            {
                success = true,
                count = cart.Sum(c => c.Quantity),
                subTotal = cart.Sum(c => c.TotalPrice)
            });
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Clear()
    {
        HttpContext.Session.Remove("Cart");
        return RedirectToAction(nameof(Index));
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
