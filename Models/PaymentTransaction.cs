using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBanQuanAoOnline.Models;

[Table("PaymentTransaction")]
public class PaymentTransaction
{
    [Key]
    public int Id { get; set; }

    [Column("MaHD")]
    [Display(Name = "Mã hóa đơn")]
    public int OrderId { get; set; }

    [StringLength(30)]
    [Display(Name = "Nhà cung cấp")]
    public string Provider { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Số tiền")]
    public decimal Amount { get; set; }

    [StringLength(30)]
    [Display(Name = "Trạng thái giao dịch")]
    public string Status { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Mã giao dịch")]
    public string TxnRef { get; set; } = string.Empty;

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(OrderId))]
    public virtual Hoadon Order { get; set; } = null!;
}
