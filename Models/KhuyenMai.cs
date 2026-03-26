using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("KhuyenMai")]
public partial class KhuyenMai
{
    [Key]
    [Display(Name = "Mã khuyến mãi")]
    public int KhuyenMaiId { get; set; }

    [StringLength(200)]
    [Display(Name = "Tên khuyến mãi")]
    public string TenKhuyenMai { get; set; } = null!;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [StringLength(50)]
    [Display(Name = "Hình thức khuyến mãi")]
    public string HinhThuc { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Giá trị khuyến mãi")]
    public decimal? GiaTri { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateOnly NgayBatDau { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateOnly NgayKetThuc { get; set; }

    [Display(Name = "Trạng thái")]
    public bool? TrangThai { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? NgayTao { get; set; }
}