using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;
using ShopBanQuanAoOnline.Services;
namespace ShopBanQuanAoOnline.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<Khachhang> _passwordHasher;
        private readonly IPaymentGatewayClient _paymentGatewayClient;
        private readonly IShippingProviderClient _shippingProviderClient;
        private readonly IConfiguration _configuration;

        public CustomersController(
            ApplicationDbContext context,
            IPasswordHasher<Khachhang> passwordHasher,
            IPaymentGatewayClient paymentGatewayClient,
            IShippingProviderClient shippingProviderClient,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _paymentGatewayClient = paymentGatewayClient;
            _shippingProviderClient = shippingProviderClient;
            _configuration = configuration;
        }

        void GetData()
        {
            ViewData["solg"] = GetCartItems().Count;
            ViewBag.danhmuc = _context.Danhmucs.ToList();
            var khEmail = HttpContext.Session.GetString("khachhang");
            if (!string.IsNullOrEmpty(khEmail))
            {
                ViewBag.khachhang = _context.Khachhangs.FirstOrDefault(k => k.Email == khEmail);
            }
        }

        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString("shopcart");
            if (jsoncart != null)
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
                return cartItems ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        void SaveCartSession(List<CartItem> list)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(list);
            session.SetString("shopcart", jsoncart);
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove("shopcart");
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 8;
            int skipAmount = (page - 1) * pageSize;

            int totalItems = await _context.Mathangs.CountAsync();

            var applicationDbContext = _context.Mathangs
                .Include(m => m.MaDmNavigation)
                .Skip(skipAmount)
                .Take(pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PageSize = pageSize;

            GetData();
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult About()
        {
            GetData();
            return View();
        }

        public async Task<IActionResult> List(int id)
        {
            var applicationDbContext = _context.Mathangs.Where(m => m.MaDm == id).Include(m => m.MaDmNavigation);
            GetData();
            ViewData["tendm"] = _context.Danhmucs.FirstOrDefault(d => d.MaDm == id).Ten;
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Tim(String tukhoa)
        {
            GetData();

            if (string.IsNullOrEmpty(tukhoa))
            {
                var allMathangs = _context.Mathangs.Include(m => m.MaDmNavigation);
                ViewBag.tukhoa = tukhoa;
                return View("Index", await allMathangs.ToListAsync());
            }

            var mathangQuery = _context.Mathangs
                .Where(m => m.Ten.Contains(tukhoa) || m.MoTa.Contains(tukhoa))
                .Include(m => m.MaDmNavigation);

            ViewBag.tukhoa = tukhoa;

            return View("Index", await mathangQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaDmNavigation)
                .Include(m => m.ProductVariants)
                .FirstOrDefaultAsync(m => m.MaMh == id);

            if (mathang == null)
            {
                return NotFound();
            }

            mathang.LuotXem = (mathang.LuotXem ?? 0) + 1;
            try
            {
                _context.Update(mathang);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            GetData();
            return View(mathang);
        }

        public async Task<IActionResult> GoiYSanPham(int take = 6)
        {
            var khEmail = HttpContext.Session.GetString("khachhang");
            if (string.IsNullOrEmpty(khEmail))
            {
                return Unauthorized();
            }

            var customer = await _context.Khachhangs.FirstOrDefaultAsync(x => x.Email == khEmail);
            if (customer == null)
            {
                return Unauthorized();
            }

            var purchasedProductIds = await _context.Hoadons
                .Where(h => h.MaKh == customer.MaKh)
                .SelectMany(h => h.Cthoadons.Select(ct => ct.MaMh))
                .Distinct()
                .ToListAsync();

            var recommendations = await _context.Mathangs
                .Where(m => !purchasedProductIds.Contains(m.MaMh))
                .OrderByDescending(m => m.LuotMua ?? 0)
                .ThenByDescending(m => m.LuotXem ?? 0)
                .Take(take)
                .Select(x => new
                {
                    x.MaMh,
                    x.Ten,
                    x.GiaBan,
                    x.HinhAnh,
                    x.MaDm
                })
                .ToListAsync();

            return Json(recommendations);
        }

        public async Task<IActionResult> DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            GetData();

            if (!tuNgay.HasValue || !denNgay.HasValue)
            {
                tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                denNgay = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }
            else
            {
                denNgay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
            }

            var query = _context.Hoadons
                .Include(hd => hd.Cthoadons)
                .ThenInclude(ct => ct.MaMhNavigation)
                .Include(hd => hd.MaKhNavigation)
                .Where(hd => hd.Ngay >= tuNgay.Value
                             && hd.Ngay <= denNgay.Value
                             && hd.TrangThai == 3);
            var listHoadon = await query.ToListAsync();


            var reportData = new DoanhThuReportViewModel { TongDoanhThu = 0, TongSoLuong = 0 };

            if (listHoadon.Any())
            {
                reportData.TongDoanhThu = (int)listHoadon
                    .SelectMany(hd => hd.Cthoadons)
                    .Sum(ct => ct.ThanhTien ?? 0);
                reportData.TongSoLuong = (int)listHoadon
                    .SelectMany(hd => hd.Cthoadons)
                    .Sum(ct => ct.SoLuong ?? 0);
            }


            ViewBag.ReportData = reportData;

            ViewData["TuNgay"] = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewData["DenNgay"] = denNgay.Value.Date.ToString("yyyy-MM-dd");

            return View(listHoadon);
        }

        public async Task<IActionResult> AddToCart(int id, int? variantId)
        {
            var mathang = await _context.Mathangs
                .FirstOrDefaultAsync(m => m.MaMh == id);

            if (mathang == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            if (mathang.SoLuong <= 0)
            {
                TempData["ErrorMessage"] = $"Sản phẩm '{mathang.Ten}' đã hết hàng!";
                return RedirectToAction(nameof(Index));
            }

            ProductVariant? selectedVariant = null;
            if (variantId.HasValue)
            {
                selectedVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Id == variantId.Value && v.ProductId == id);

                if (selectedVariant == null)
                {
                    TempData["ErrorMessage"] = "Biến thể sản phẩm không hợp lệ.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                if (selectedVariant.Stock <= 0)
                {
                    TempData["ErrorMessage"] = $"Biến thể {selectedVariant.Size}/{selectedVariant.Color} đã hết hàng.";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }

            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id && p.ProductVariantId == variantId);

            if (item != null)
            {
                var availableStock = selectedVariant?.Stock ?? mathang.SoLuong ?? 0;
                if ((item.SoLuong + 1) > availableStock)
                {
                    TempData["ErrorMessage"] = $"Số lượng sản phẩm '{mathang.Ten}' không đủ ({availableStock} cái).";
                    return RedirectToAction(nameof(ViewCart));
                }
                item.SoLuong++;
            }
            else
            {
                cart.Add(new CartItem()
                {
                    // Chỉ lưu thông tin cần thiết để tránh vòng lặp tham chiếu khi serialize session cart
                    MatHang = new Mathang
                    {
                        MaMh = mathang.MaMh,
                        Ten = mathang.Ten,
                        GiaBan = mathang.GiaBan,
                        HinhAnh = mathang.HinhAnh,
                        SoLuong = mathang.SoLuong
                    },

                    ProductVariantId = selectedVariant?.Id,
                    VariantLabel = selectedVariant == null ? null : $"{selectedVariant.Size} / {selectedVariant.Color}",
                    SoLuong = 1
                });
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult ViewCart()
        {
            GetData();
            return View(GetCartItems());
        }

        public IActionResult RemoveItem(int id, int? variantId)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id && p.ProductVariantId == variantId);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        public async Task<IActionResult> UpdateItem(int id, int quantity, int? variantId)
        {
            var cart = GetCartItems();
            var item = cart.Find(p => p.MatHang.MaMh == id && p.ProductVariantId == variantId);
            if (item != null)
            {
                int availableStock;
                if (variantId.HasValue)
                {
                    var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId.Value && v.ProductId == id);
                    availableStock = variant?.Stock ?? 0;
                }
                else
                {
                    var product = await _context.Mathangs.FirstOrDefaultAsync(p => p.MaMh == id);
                    availableStock = product?.SoLuong ?? 0;
                }

                if (quantity <= 0) quantity = 1;
                if (quantity > availableStock) quantity = availableStock;
                item.SoLuong = quantity;
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(ViewCart));
        }

        public async Task<IActionResult> CheckOut()
        {
            var khEmail = HttpContext.Session.GetString("khachhang");
            if (string.IsNullOrEmpty(khEmail))
            {
                return RedirectToAction(nameof(Login));
            }

            GetData();
            var khachhang = ViewBag.khachhang as Khachhang;

            var defaultAddress = await _context.Diachis
                                .FirstOrDefaultAsync(dc => dc.MaKh == khachhang.MaKh && dc.MacDinh == 1);

            ViewBag.DefaultAddress = defaultAddress;

            return View(GetCartItems());
        }

        [HttpPost, ActionName("CreateBill")]
        public async Task<IActionResult> CreateBill(string email, string hoten, string dienthoai, string diachi, bool saveAddress, string paymentMethod = "COD", string shippingMethod = "FAST")
        {
            var cart = GetCartItems();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng rỗng! Vui lòng thêm sản phẩm.";
                return RedirectToAction(nameof(ViewCart));
            }

            var itemsToUpdate = new List<Mathang>();

            foreach (var item in cart)
            {
                var mathang = await _context.Mathangs.FindAsync(item.MatHang.MaMh);
                if (mathang == null || mathang.SoLuong < item.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm **'{item.MatHang.Ten}'** đã hết hàng hoặc không đủ số lượng ({item.SoLuong} cái) trong kho. Vui lòng cập nhật giỏ hàng.";
                    return RedirectToAction(nameof(ViewCart));
                }

                if (item.ProductVariantId.HasValue)
                {
                    var variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId.Value && v.ProductId == item.MatHang.MaMh);
                    if (variant == null || variant.Stock < item.SoLuong)
                    {
                        TempData["ErrorMessage"] = $"Biến thể **'{item.VariantLabel}'** của sản phẩm '{item.MatHang.Ten}' không đủ số lượng.";
                        return RedirectToAction(nameof(ViewCart));
                    }
                }


                mathang.LuotMua = (mathang.LuotMua ?? 0) + item.SoLuong;
                mathang.SoLuong = (short)((mathang.SoLuong ?? 0) - item.SoLuong);
                itemsToUpdate.Add(mathang);
            }

            var kh = _context.Khachhangs.FirstOrDefault(k => k.Email == HttpContext.Session.GetString("khachhang"));
            var hd = new Hoadon();
            hd.Ngay = DateTime.Now;
            hd.MaKh = kh.MaKh;
            hd.TrangThai = 0;
            _context.Add(hd);
            await _context.SaveChangesAsync();


            int tongtien = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                var item = cart[i];
                var mathang = itemsToUpdate[i];

                int thanhtien = (item.MatHang.GiaBan ?? 0) * item.SoLuong;
                tongtien += thanhtien;

                var ct = new Cthoadon();
                ct.MaHd = hd.MaHd;
                ct.MaMh = item.MatHang.MaMh;
                ct.MaBienThe = item.ProductVariantId;
                ct.DonGia = item.MatHang.GiaBan;
                ct.SoLuong = (short)item.SoLuong;
                ct.ThanhTien = thanhtien;
                _context.Add(ct);

                _context.Update(mathang);

                if (item.ProductVariantId.HasValue)
                {
                    var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == item.ProductVariantId.Value);
                    if (variant != null)
                    {
                        variant.Stock -= item.SoLuong;
                        _context.Update(variant);
                    }
                }
            }

            hd.TongTien = tongtien;
            _context.Update(hd);
            await _context.SaveChangesAsync();

            var shipmentCode = await _shippingProviderClient.CreateShipmentAsync(hd, diachi);
            var shippingMethodText = shippingMethod.Equals("STANDARD", StringComparison.OrdinalIgnoreCase)
               ? "Tiêu chuẩn (2-4 ngày)"
               : "Nhanh (1-2 ngày)";

            var paymentUrl = string.Empty;
            var bankQrUrl = string.Empty;
            var transferContent = $"DH{hd.MaHd}";
            if (paymentMethod.Equals("VNPAY", StringComparison.OrdinalIgnoreCase))
            {
                paymentUrl = await _paymentGatewayClient.CreatePaymentUrlAsync(
                    hd,
                    tongtien,
                    Url.Action(nameof(CustomerInfo), "Customers", null, Request.Scheme) ?? string.Empty);

                var paymentTransaction = new PaymentTransaction
                {
                    OrderId = hd.MaHd,
                    Provider = "VNPAY",
                    Amount = tongtien,
                    Status = "Pending",
                    TxnRef = $"VNPAY-{hd.MaHd}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
                };
                _context.PaymentTransactions.Add(paymentTransaction);
                await _context.SaveChangesAsync();
            }
            else if (paymentMethod.Equals("BANK_QR", StringComparison.OrdinalIgnoreCase))
            {
                var bankBin = _configuration["ExternalServices:BankQr:BankBin"] ?? "970422";
                var accountNumber = _configuration["ExternalServices:BankQr:AccountNumber"] ?? "0000000000";
                var accountName = _configuration["ExternalServices:BankQr:AccountName"] ?? "CHU TAI KHOAN";
                var template = _configuration["ExternalServices:BankQr:Template"] ?? "compact2";

                bankQrUrl = $"https://img.vietqr.io/image/{bankBin}-{accountNumber}-{template}.png" +
                            $"?amount={tongtien}" +
                            $"&addInfo={Uri.EscapeDataString(transferContent)}" +
                            $"&accountName={Uri.EscapeDataString(accountName)}";

                var paymentTransaction = new PaymentTransaction
                {
                    OrderId = hd.MaHd,
                    Provider = "BANK_QR",
                    Amount = tongtien,
                    Status = "Pending",
                    TxnRef = $"BANKQR-{hd.MaHd}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
                };
                _context.PaymentTransactions.Add(paymentTransaction);
                await _context.SaveChangesAsync();
            }

            if (saveAddress)
            {
                var oldDefault = await _context.Diachis.FirstOrDefaultAsync(d => d.MaKh == kh.MaKh && d.MacDinh == 1);
                if (oldDefault != null)
                {
                    oldDefault.MacDinh = 0;
                    _context.Update(oldDefault);
                }

                var newAddress = new Diachi
                {
                    MaKh = kh.MaKh,
                    DiaChi1 = diachi,
                    MacDinh = 1,
                };
                _context.Add(newAddress);
                await _context.SaveChangesAsync();
            }

            ClearCart();
            GetData();

            TempData["SuccessMessage"] = $"Đặt hàng thành công! Đơn hàng #{hd.MaHd} đã **trừ kho** và đang chờ xử lý. Vận chuyển: {shippingMethodText}. Mã vận đơn: {shipmentCode}";
            TempData["ShippingMethodText"] = shippingMethodText;
            if (!string.IsNullOrEmpty(paymentUrl))
            {
                TempData["PaymentUrl"] = paymentUrl;
            }
            if (!string.IsNullOrEmpty(bankQrUrl))
            {
                TempData["BankQrUrl"] = bankQrUrl;
                TempData["TransferContent"] = transferContent;
                TempData["BankAccount"] = _configuration["ExternalServices:BankQr:AccountNumber"] ?? "0000000000";
                TempData["BankName"] = _configuration["ExternalServices:BankQr:BankName"] ?? "Ngân hàng của bạn";
                TempData["AccountName"] = _configuration["ExternalServices:BankQr:AccountName"] ?? "CHU TAI KHOAN";
            }
            return View(hd);
        }

        public IActionResult Register()
        {
            GetData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Khachhang kh, string diachidangky)
        {
            GetData();

            if (!ModelState.IsValid)
                return View(kh);

            if (await _context.Khachhangs.AnyAsync(k => k.Email == kh.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(kh);
            }

            kh.MatKhau = _passwordHasher.HashPassword(kh, kh.MatKhau);

            _context.Add(kh);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(diachidangky))
            {
                Diachi dc = new Diachi
                {
                    MaKh = kh.MaKh,
                    DiaChi1 = diachidangky,
                    MacDinh = 1
                };
                _context.Add(dc);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            GetData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string matkhau)
        {
            var kh = await _context.Khachhangs
                .FirstOrDefaultAsync(m => m.Email == email);

            if (kh != null && _passwordHasher.VerifyHashedPassword(kh, kh.MatKhau, matkhau) == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetString("khachhang", kh.Email);
                return RedirectToAction(nameof(CustomerInfo));
            }

            TempData["ErrorMessage"] = "Email hoặc mật khẩu không chính xác.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> CustomerInfo()
        {
            GetData();
            var kh = _context.Khachhangs.FirstOrDefault(k => k.Email == HttpContext.Session.GetString("khachhang"));

            var hd = _context.Hoadons
                .Include(ct => ct.Cthoadons)
                .ThenInclude(c => c.MaMhNavigation)
                .Include(ct => ct.Cthoadons)
                .ThenInclude(c => c.MaBienTheNavigation)
                .Where(h => h.MaKh == kh.MaKh);

            return View(await hd.ToListAsync());
        }

        public IActionResult Signout()
        {
            HttpContext.Session.SetString("khachhang", "");
            GetData();
            return RedirectToAction("Index");
        }
    }
}