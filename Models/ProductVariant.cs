using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ShopBanQuanAoOnline.Models;

[Table("ProductVariant")]
public class ProductVariant
{
    [Key]
    public int Id { get; set; }

    [Column("MaMH")]
    [Display(Name = "Mã mặt hàng")]
    public int ProductId { get; set; }

    [StringLength(20)]
    [Display(Name = "Kích thước")]
    public string Size { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Màu sắc")]
    public string Color { get; set; } = string.Empty;

    [Display(Name = "Số lượng tồn")]
    public int Stock { get; set; }

    [StringLength(50)]
    [Display(Name = "Mã SKU")]
    public string Sku { get; set; } = string.Empty;

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ProductId))]
    public virtual Mathang Product { get; set; } = null!;

    [InverseProperty("MaBienTheNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();
}
