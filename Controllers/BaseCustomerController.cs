using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ShopBanQuanAoOnline.Controllers
{   
    public class BaseCustomerController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseCustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CustomerInfo()
        {
            GetData();
            
            var kh = _context.Khachhangs.FirstOrDefault(k => k.Email == HttpContext.Session.GetString("khachhang"));

            var hd = _context.Hoadons
                .Include(ct => ct.Cthoadons)
                .ThenInclude(c => c.MaMhNavigation)
                .Where(h => h.MaKh == kh.MaKh);

            return View(await hd.ToListAsync());
        }
        public IActionResult Signout()
        {
            HttpContext.Session.SetString("khachhang", "");
            GetData();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> List(int id)
        {            
            var applicationDbContext = _context.Mathangs.Where(m => m.MaDm == id).Include(m => m.MaDmNavigation);

            GetData();

            ViewData["tendm"] = _context.Danhmucs.FirstOrDefault(d => d.MaDm == id)?.Ten;
                      
            return View("~/Views/Customers/List.cshtml", await applicationDbContext.ToListAsync());
        }
        protected List<CartItem> GetCartItems()
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

        protected void GetData()
        {            
            ViewData["solg"] = GetCartItems().Count;

            ViewBag.danhmuc = _context.Danhmucs.ToList();

            var khEmail = HttpContext.Session.GetString("khachhang");
            if (!string.IsNullOrEmpty(khEmail))
            {
                var khachhang = _context.Khachhangs.FirstOrDefault(k => k.Email == khEmail);
                if (khachhang != null)
                {
                    ViewBag.khachhang = khachhang;
                }
            }
        }
    }
}