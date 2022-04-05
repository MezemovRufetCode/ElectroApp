
using ElectroApp.DAL;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM
            {
                IntroSliders=_context.IntroSliders.ToList(),
                Products=_context.Products.Include(p=>p.ProductCategories).ThenInclude(pc=>pc.Category).Include(p=>p.ProductImages).
                Include(p=>p.Brand).Include(p=>p.Campaign).ToList()
            };
            return View(homeVM);
        }
    }
}
