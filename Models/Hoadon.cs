using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("HOADON")]
public partial class Hoadon
{
    [Key]
    [Column("MaHD")]
    [Display(Name = "Mã hóa đơn")]
    public int MaHd { get; set; }

    [Column(TypeName = "datetime")]
    [Display(Name = "Ngày lập")]
    public DateTime? Ngay { get; set; }

    [Display(Name = "Tổng tiền")]
    public int? TongTien { get; set; }

    [Column("MaKH")]
    [Display(Name = "Mã khách hàng")]
    public int MaKh { get; set; }

    [Display(Name = "Trạng thái")]
    public int? TrangThai { get; set; }

    [InverseProperty("MaHdNavigation")]
    [Display(Name = "Chi tiết hóa đơn")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [ForeignKey("MaKh")]
    [InverseProperty("Hoadons")]
    [Display(Name = "Khách hàng")]
    public virtual Khachhang MaKhNavigation { get; set; } = null!;
}