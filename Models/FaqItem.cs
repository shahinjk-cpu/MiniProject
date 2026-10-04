namespace PetShopApp.Models;

public class FaqItem : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
