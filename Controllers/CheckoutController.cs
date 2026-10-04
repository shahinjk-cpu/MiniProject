using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Extensions;
using PetShopApp.Models;
using PetShopApp.Models.Enums;
using PetShopApp.ViewModels;

namespace PetShopApp.Controllers;

public class CheckoutController : Controller
{
    private readonly AppDbContext _context;

    public CheckoutController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Checkout";
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty. Please add products before checking out.";
            return RedirectToAction("Index", "Cart");
        }

        var subtotal = cart.Sum(c => c.TotalPrice);
        var shipping = subtotal > 50 ? 0m : 10.00m;

        var model = new CheckoutVM
        {
            Items = cart,
            SubTotal = subtotal,
            ShippingFee = shipping,
            TotalAmount = subtotal + shipping
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutVM model)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        if (!cart.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        model.Items = cart;
        model.SubTotal = cart.Sum(c => c.TotalPrice);
        model.ShippingFee = model.SubTotal > 50 ? 0m : 10.00m;
        model.TotalAmount = model.SubTotal + model.ShippingFee;

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var orderCode = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";

        var order = new Order
        {
            OrderCode = orderCode,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            CompanyName = model.CompanyName?.Trim(),
            AddressLine1 = model.AddressLine1.Trim(),
            AddressLine2 = model.AddressLine2?.Trim(),
            City = model.City.Trim(),
            PostalCode = model.PostalCode.Trim(),
            Phone = model.Phone.Trim(),
            Email = model.Email.Trim(),
            OrderNotes = model.OrderNotes?.Trim(),
            PaymentMethod = model.PaymentMethod,
            Status = OrderStatus.Pending,
            SubTotal = model.SubTotal,
            ShippingFee = model.ShippingFee,
            TotalAmount = model.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in cart)
        {
            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductThumbnail = item.ImageUrl,
                UnitPrice = item.Price,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice
            });

            // Stok sayını azalt
            var prod = await _context.Products.FindAsync(item.ProductId);
            if (prod != null)
            {
                prod.StockQuantity = Math.Max(0, prod.StockQuantity - item.Quantity);
            }
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Səbəti təmizlə
        HttpContext.Session.Remove("Cart");

        return RedirectToAction(nameof(ThankYou), new { orderCode = order.OrderCode });
    }

    public async Task<IActionResult> ThankYou(string orderCode)
    {
        ViewData["Title"] = "Order Confirmation";
        if (string.IsNullOrWhiteSpace(orderCode)) return RedirectToAction("Index", "Home");

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

        if (order == null) return NotFound();

        return View(order);
    }
}
