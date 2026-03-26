using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("DIACHI")]
public partial class Diachi
{
    [Key]
    [Column("MaDC")]
    [Display(Name = "Mã địa chỉ")]
    public int MaDc { get; set; }

    [Column("MaKH")]
    [Display(Name = "Mã khách hàng")]
    public int MaKh { get; set; }

    [Column("DiaChi")]
    [StringLength(100)]
    [Display(Name = "Địa chỉ")]
    public string DiaChi1 { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    [Display(Name = "Phường / Xã")]
    public string? PhuongXa { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    [Display(Name = "Quận / Huyện")]
    public string? QuanHuyen { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    [Display(Name = "Tỉnh / Thành phố")]
    public string? TinhThanh { get; set; }

    [Display(Name = "Địa chỉ mặc định")]
    public int? MacDinh { get; set; }

    [ForeignKey("MaKh")]
    [InverseProperty("Diachis")]
    [Display(Name = "Khách hàng")]
    public virtual Khachhang MaKhNavigation { get; set; } = null!;
}