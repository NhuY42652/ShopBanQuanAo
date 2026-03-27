namespace ShopBanQuanAoOnline.Models
{
    public class CartItem
    {
        public Mathang MatHang { get; set; } = null!;
        public int? ProductVariantId { get; set; }
        public string? VariantLabel { get; set; }
        public int SoLuong { get; set; }
    }
}