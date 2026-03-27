using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Controllers
{
    public class HoadonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HoadonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hoadons.Include(h => h.MaKhNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons

                .Include(h => h.MaKhNavigation)
                    .ThenInclude(kh => kh.Diachis)
                .Include(hd => hd.Cthoadons)
                    .ThenInclude(ct => ct.MaMhNavigation)
                .Include(hd => hd.Cthoadons)
                    .ThenInclude(ct => ct.MaBienTheNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.Khachhangs, "MaKh", "MaKh");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHd,Ngay,TongTien,MaKh,TrangThai")] Hoadon hoadon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhangs, "MaKh", "MaKh", hoadon.MaKh);
            return View(hoadon);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhangs, "MaKh", "MaKh", hoadon.MaKh);
            return View(hoadon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,Ngay,TongTien,MaKh,TrangThai")] Hoadon hoadon)
        {
            if (id != hoadon.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoadonExists(hoadon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.Khachhangs, "MaKh", "MaKh", hoadon.MaKh);
            return View(hoadon);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon != null)
            {
                _context.Hoadons.Remove(hoadon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       
        private bool HoadonExists(int id)
        {
            return _context.Hoadons.Any(e => e.MaHd == id);
        }

        private async Task HoanSoLuongKho(ICollection<Cthoadon> chiTietDonHang)
        {
            foreach (var ct in chiTietDonHang)
            {
                var mathang = await _context.Mathangs.FindAsync(ct.MaMh);

                if (mathang != null)
                {                    
                    mathang.SoLuong = (short)((mathang.SoLuong ?? 0) + (ct.SoLuong ?? 0));
                    _context.Update(mathang);
                }
            }
            await _context.SaveChangesAsync();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, int trangThaiMoi, string returnUrl)
        {
            var hoadon = await _context.Hoadons.FindAsync(id);

            if (hoadon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn.";
                return Redirect(returnUrl ?? nameof(Index));
            }

            hoadon.TrangThai = trangThaiMoi;

            try
            {
                _context.Update(hoadon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{id} thành công!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoadonExists(id)) { return NotFound(); } else { throw; }
            }

            return Redirect(returnUrl ?? nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDon(int id, string returnUrl)
        {
            var hoadon = await _context.Hoadons
                .Include(hd => hd.Cthoadons)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            if (hoadon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn.";
                return Redirect(returnUrl ?? nameof(Index));
            }

            if (hoadon.TrangThai == 3)
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng này vì nó đã được giao thành công.";
                return Redirect(returnUrl ?? nameof(Index));
            }

            if (hoadon.TrangThai == -1)
            {
                TempData["ErrorMessage"] = "Đơn hàng này đã bị hủy trước đó.";
                return Redirect(returnUrl ?? nameof(Index));
            }

            if (hoadon.TrangThai != -1)
            {
                await HoanSoLuongKho(hoadon.Cthoadons);
            }
            hoadon.TrangThai = -1;

            try
            {
                _context.Update(hoadon);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Hóa đơn #{id} đã được hủy thành công và **số lượng kho đã được hoàn lại**!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoadonExists(id)) { return NotFound(); } else { throw; }
            }

            return Redirect(returnUrl ?? nameof(Index));
        }
    }
}
