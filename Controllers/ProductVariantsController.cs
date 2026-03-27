using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Controllers;

public class ProductVariantsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductVariantsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? maMh)
    {
        var query = _context.ProductVariants.Include(v => v.Product).AsQueryable();
        if (maMh.HasValue)
        {
            query = query.Where(v => v.ProductId == maMh.Value);
            ViewBag.SelectedProductId = maMh.Value;
        }

        return View(await query.OrderBy(v => v.ProductId).ThenBy(v => v.Size).ThenBy(v => v.Color).ToListAsync());
    }

    public IActionResult Create(int? maMh)
    {
        ViewData["ProductId"] = new SelectList(_context.Mathangs, "MaMh", "Ten", maMh);
        return View(new ProductVariant { ProductId = maMh ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductId,Size,Color,Stock,Sku")] ProductVariant productVariant)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = new SelectList(_context.Mathangs, "MaMh", "Ten", productVariant.ProductId);
            return View(productVariant);
        }

        _context.ProductVariants.Add(productVariant);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { maMh = productVariant.ProductId });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var productVariant = await _context.ProductVariants.FindAsync(id);
        if (productVariant == null) return NotFound();

        ViewData["ProductId"] = new SelectList(_context.Mathangs, "MaMh", "Ten", productVariant.ProductId);
        return View(productVariant);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Size,Color,Stock,Sku")] ProductVariant productVariant)
    {
        if (id != productVariant.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = new SelectList(_context.Mathangs, "MaMh", "Ten", productVariant.ProductId);
            return View(productVariant);
        }

        try
        {
            _context.Update(productVariant);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.ProductVariants.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToAction(nameof(Index), new { maMh = productVariant.ProductId });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var productVariant = await _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (productVariant == null) return NotFound();

        return View(productVariant);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var productVariant = await _context.ProductVariants.FindAsync(id);
        if (productVariant == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var maMh = productVariant.ProductId;
        _context.ProductVariants.Remove(productVariant);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { maMh });
    }
}
