using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetShopApp.Models;

namespace PetShopApp.Areas.Admin.ViewModels;

public class DashboardVM
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrdersCount { get; set; }
    public int UnreadMessagesCount { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
    public List<Product> LowStockProducts { get; set; } = new();
}

public class ProductFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Məhsulun adı mütləqdir")]
    [StringLength(200, ErrorMessage = "Maksimum 200 simvol")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? AdditionalInfo { get; set; }

    [Required(ErrorMessage = "Qiymət mütləqdir")]
    [Range(0.01, 100000, ErrorMessage = "Qiymət 0-dan böyük olmalıdır")]
    public decimal Price { get; set; }

    public decimal? OldPrice { get; set; }

    [Required(ErrorMessage = "SKU mütləqdir")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Stok miqdarı mütləqdir")]
    [Range(0, 100000, ErrorMessage = "Stok mənfi ola bilməz")]
    public int StockQuantity { get; set; } = 10;

    [Required(ErrorMessage = "Kateqoriya seçilməlidir")]
    public int CategoryId { get; set; }

    public string MainImageUrl { get; set; } = "images/item1.jpg";

    public bool IsClothing { get; set; }
    public bool IsFoodie { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;

    public List<SelectListItem> CategoryOptions { get; set; } = new();
}

public class CategoryFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kateqoriya adı mütləqdir")]
    [StringLength(100, ErrorMessage = "Maksimum 100 simvol")]
    public string Name { get; set; } = string.Empty;

    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class AdminLoginVM
{
    [Required(ErrorMessage = "Email daxil edilməlidir")]
    [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifrə daxil edilməlidir")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
