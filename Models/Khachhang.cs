using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("KHACHHANG")]
public partial class Khachhang
{
    [Key]
    [Column("MaKH")]
    [Display(Name = "Mã khách hàng")]
    public int MaKh { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100)]
    [Display(Name = "Tên khách hàng")]
    public string Ten { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [StringLength(20)]
    [Unicode(false)]
    [Display(Name = "Số điện thoại")]
    public string? DienThoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(50)]
    [Unicode(false)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [StringLength(500)]
    [Unicode(false)]
    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }

    [InverseProperty("MaKhNavigation")]
    [Display(Name = "Danh sách địa chỉ")]
    public virtual ICollection<Diachi> Diachis { get; set; } = new List<Diachi>();

    [InverseProperty("MaKhNavigation")]
    [Display(Name = "Danh sách hóa đơn")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}