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
    public class MathangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MathangsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mathangs.Include(m => m.MaDmNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaDmNavigation)
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

            return View(mathang);
        }

        public string? Upload(IFormFile file)
        {
            string fn = null;

            if (file != null)
            {                
                fn = Guid.NewGuid().ToString() + "_" + file.FileName;
                                
                var path = $"wwwroot\\images\\{fn}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return fn;
        }

        public IActionResult Create()
        {
            ViewBag.MaDmList = new SelectList(_context.Danhmucs, "MaDm", "Ten");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file,
            [Bind("Ten,MoTa,GiaGoc,GiaBan,SoLuong,MaDm,LuotXem,LuotMua")] Mathang mathang) 
        {         
            if (file != null)
            {
                mathang.HinhAnh = Upload(file);
            }
            else
            {
               
            }

            if (mathang.LuotXem == null) mathang.LuotXem = 0;
            if (mathang.LuotMua == null) mathang.LuotMua = 0;

            if (ModelState.IsValid)
            {
                _context.Add(mathang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaDmList = new SelectList(_context.Danhmucs, "MaDm", "Ten", mathang.MaDm);
            return View(mathang);
        }
                
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang == null)
            {
                return NotFound();
            }
  
            ViewBag.MaDm = new SelectList(_context.Danhmucs, "MaDm", "Ten", mathang.MaDm);
            return View(mathang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile? file, [Bind("MaMh,Ten,GiaGoc,GiaBan,SoLuong,MoTa,HinhAnh,MaDm,LuotXem,LuotMua")] Mathang mathang)
        {
            if (id != mathang.MaMh)
            {
                return NotFound();
            }

            ModelState.Remove("MaDmNavigation");
            ModelState.Remove("HinhAnh");
            ModelState.Remove("file");

            var mathangCu = await _context.Mathangs.AsNoTracking().FirstOrDefaultAsync(x => x.MaMh == id);

            if (file != null)
            {
                mathang.HinhAnh = Upload(file);
            }
            else
            {
                if (mathangCu != null)
                {
                    mathang.HinhAnh = mathangCu.HinhAnh;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mathang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MathangExists(mathang.MaMh))
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

            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", mathang.MaDm);
            return View(mathang);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mathang = await _context.Mathangs
                .Include(m => m.MaDmNavigation)
                .FirstOrDefaultAsync(m => m.MaMh == id);
            if (mathang == null)
            {
                return NotFound();
            }

            return View(mathang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mathang = await _context.Mathangs.FindAsync(id);
            if (mathang != null)
            {
                _context.Mathangs.Remove(mathang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MathangExists(int id)
        {
            return _context.Mathangs.Any(e => e.MaMh == id);
        }
    }
}