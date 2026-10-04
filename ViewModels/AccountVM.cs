using System.ComponentModel.DataAnnotations;
using PetShopApp.Models;

namespace PetShopApp.ViewModels;

public class LoginVM
{
    [Required(ErrorMessage = "Email daxil edilməlidir")]
    [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifrə daxil edilməlidir")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class RegisterVM
{
    [Required(ErrorMessage = "Ad və soyad daxil edilməlidir")]
    [StringLength(100, ErrorMessage = "Ad maksimum 100 simvol ola bilər")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email daxil edilməlidir")]
    [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifrə daxil edilməlidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən azı 6 simvoldan ibarət olmalıdır")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifrənin təkrarı daxil edilməlidir")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifrələr uyğun gəlmir")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AccountPageVM
{
    public bool IsAuthenticated { get; set; }
    public string ActiveTab { get; set; } = "login"; // "login" or "register"
    public LoginVM Login { get; set; } = new();
    public RegisterVM Register { get; set; } = new();

    // Logged in user info & orders
    public AppUser? User { get; set; }
    public List<Order> Orders { get; set; } = new();
}
