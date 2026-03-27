using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBanQuanAoOnline.Models;

[Table("Review")]
public class Review
{
    [Key]
    public int Id { get; set; }

    [Column("MaMH")]
    [Display(Name = "Mã mặt hàng")]
    public int ProductId { get; set; }

    [Column("MaKH")]
    [Display(Name = "Mã khách hàng")]
    public int CustomerId { get; set; }

    [Range(1, 5)]
    [Display(Name = "Điểm đánh giá")]
    public int Rating { get; set; }

    [StringLength(500)]
    [Display(Name = "Nội dung đánh giá")]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ProductId))]
    public virtual Mathang Product { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public virtual Khachhang Customer { get; set; } = null!;
}
