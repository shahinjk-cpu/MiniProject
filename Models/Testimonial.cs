namespace PetShopApp.Models;

public class Testimonial : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Pet Owner";
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public string AvatarUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
