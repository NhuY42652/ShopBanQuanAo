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
    public class CthoadonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CthoadonsController(ApplicationDbContext context)
        {
            _context = context;
        }
               
        public async Task<IActionResult> Index()
        {           
            var applicationDbContext = _context.Cthoadons
                .Include(c => c.MaMhNavigation) 
                .Include(c => c.MaHdNavigation) 
                    .ThenInclude(h => h.MaKhNavigation); 

            return View(await applicationDbContext.ToListAsync());

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons
                .Include(c => c.MaMhNavigation) 
                .Include(c => c.MaHdNavigation) 
                    .ThenInclude(h => h.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaCthd == id);

            if (cthoadon == null)
            {
                return NotFound();
            }

            return View(cthoadon);
        }

        public IActionResult Create()
        {            
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd");
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh");
            return View();
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCthd,MaHd,MaMh,DonGia,SoLuong,ThanhTien")] Cthoadon cthoadon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cthoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons.FindAsync(id);
            if (cthoadon == null)
            {
                return NotFound();
            }
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCthd,MaHd,MaMh,DonGia,SoLuong,ThanhTien")] Cthoadon cthoadon)
        {
            if (id != cthoadon.MaCthd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cthoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CthoadonExists(cthoadon.MaCthd))
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
            ViewData["MaHd"] = new SelectList(_context.Hoadons, "MaHd", "MaHd", cthoadon.MaHd);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", cthoadon.MaMh);
            return View(cthoadon);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthoadon = await _context.Cthoadons
                .Include(c => c.MaMhNavigation) 
                .Include(c => c.MaHdNavigation) 
                    .ThenInclude(h => h.MaKhNavigation) 
                .FirstOrDefaultAsync(m => m.MaCthd == id);

            if (cthoadon == null)
            {
                return NotFound();
            }

            return View(cthoadon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cthoadon = await _context.Cthoadons.FindAsync(id);
            if (cthoadon != null)
            {
                _context.Cthoadons.Remove(cthoadon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthoadonExists(int id)
        {
            return _context.Cthoadons.Any(e => e.MaCthd == id);
        }

    }
}