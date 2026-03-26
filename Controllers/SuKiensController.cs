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
    public class SuKiensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuKiensController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.SuKiens.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suKien = await _context.SuKiens
                .FirstOrDefaultAsync(m => m.SuKienId == id);
            if (suKien == null)
            {
                return NotFound();
            }

            return View(suKien);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SuKienId,TenSuKien,MoTa,NgayBatDau,NgayKetThuc,TrangThai,NgayTao")] SuKien suKien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suKien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suKien);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suKien = await _context.SuKiens.FindAsync(id);
            if (suKien == null)
            {
                return NotFound();
            }
            return View(suKien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SuKienId,TenSuKien,MoTa,NgayBatDau,NgayKetThuc,TrangThai,NgayTao")] SuKien suKien)
        {
            if (id != suKien.SuKienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suKien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuKienExists(suKien.SuKienId))
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
            return View(suKien);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suKien = await _context.SuKiens
                .FirstOrDefaultAsync(m => m.SuKienId == id);
            if (suKien == null)
            {
                return NotFound();
            }

            return View(suKien);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suKien = await _context.SuKiens.FindAsync(id);
            if (suKien != null)
            {
                _context.SuKiens.Remove(suKien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuKienExists(int id)
        {
            return _context.SuKiens.Any(e => e.SuKienId == id);
        }
    }
}