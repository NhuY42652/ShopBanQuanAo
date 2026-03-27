using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("MATHANG")]
public partial class Mathang
{
    [Key]
    [Column("MaMH")]
    [Display(Name = "Mã mặt hàng")]
    public int MaMh { get; set; }

    [StringLength(100)]
    [Display(Name = "Tên mặt hàng")]
    public string Ten { get; set; } = null!;

    [Display(Name = "Giá gốc")]
    public int? GiaGoc { get; set; }

    [Display(Name = "Giá bán")]
    public int? GiaBan { get; set; }

    [Display(Name = "Số lượng")]
    public short? SoLuong { get; set; }

    [StringLength(1000)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Hình ảnh")]
    public string? HinhAnh { get; set; }

    [Column("MaDM")]
    [Display(Name = "Mã danh mục")]
    public int MaDm { get; set; }

    [Display(Name = "Lượt xem")]
    public int? LuotXem { get; set; }

    [Display(Name = "Lượt mua")]
    public int? LuotMua { get; set; }

    [InverseProperty("MaMhNavigation")]
    [Display(Name = "Chi tiết hóa đơn")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [InverseProperty("Product")]
    [Display(Name = "Biến thể sản phẩm")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    [InverseProperty("Product")]
    [Display(Name = "Đánh giá sản phẩm")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("MaDm")]
    [InverseProperty("Mathangs")]
    [ValidateNever]
    [Display(Name = "Danh mục")]
    public virtual Danhmuc MaDmNavigation { get; set; } = null!;
}