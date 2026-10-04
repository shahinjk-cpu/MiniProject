using System.ComponentModel.DataAnnotations;
using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class CheckoutVM
{
    public List<CartItem> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; } = 0m;
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Street address is required")]
    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "Town / City is required")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Zip / Postal code is required")]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    public string? OrderNotes { get; set; }

    public string PaymentMethod { get; set; } = "CashOnDelivery";
}
