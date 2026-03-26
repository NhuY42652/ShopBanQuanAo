using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Models;

[Table("SuKien")]
public partial class SuKien
{
    [Key]
    [Display(Name = "Mã sự kiện")]
    public int SuKienId { get; set; }

    [StringLength(200)]
    [Display(Name = "Tên sự kiện")]
    public string TenSuKien { get; set; } = null!;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

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