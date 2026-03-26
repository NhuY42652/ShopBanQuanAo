using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;

namespace ShopBanQuanAoOnline.Controllers
{   
    public class CustomersSuKienController : BaseCustomerController
    {
       
        public CustomersSuKienController(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            GetData(); 
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

            GetData(); 
            return View(suKien);
        }
    }
}