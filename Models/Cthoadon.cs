using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("CTHOADON")]
public partial class Cthoadon
{
    [Key]
    [Column("MaCTHD")]
    [Display(Name = "Mã CTHD")]
    public int MaCthd { get; set; }

    [Column("MaHD")]
    [Display(Name = "Mã Hóa Đơn")]
    public int MaHd { get; set; }

    [Column("MaMH")]
    [Display(Name = "Mã Mặt Hàng")]
    public int MaMh { get; set; }

    [Column("MaBienThe")]
    [Display(Name = "Mã biến thể")]
    public int? MaBienThe { get; set; }

    [Display(Name = "Đơn Giá")]
    public int? DonGia { get; set; }

    [Display(Name = "Số Lượng")]
    public short? SoLuong { get; set; }

    [Display(Name = "Thành Tiền")]
    public int? ThanhTien { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("Cthoadons")]
    [Display(Name = "Hóa Đơn")]
    public virtual Hoadon MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaMh")]
    [InverseProperty("Cthoadons")]
    [Display(Name = "Mặt Hàng")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;
    
    [ForeignKey("MaBienThe")]
    [InverseProperty("Cthoadons")]
    [Display(Name = "Biến thể")]
    public virtual ProductVariant? MaBienTheNavigation { get; set; }
}