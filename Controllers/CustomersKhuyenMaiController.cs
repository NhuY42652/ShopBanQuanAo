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
    public class CustomersKhuyenMaiController : BaseCustomerController
    {
        
        public CustomersKhuyenMaiController(ApplicationDbContext context) : base(context)
        {           
        }

       
        public async Task<IActionResult> Index()
        {
            GetData();
            return View(await _context.KhuyenMais.ToListAsync()); 
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuyenMai = await _context.KhuyenMais
                .FirstOrDefaultAsync(m => m.KhuyenMaiId == id);

            if (khuyenMai == null)
            {
                return NotFound();
            }

            GetData();
            return View(khuyenMai);
        }
    }
}