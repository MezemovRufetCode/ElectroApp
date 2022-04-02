using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 12);
            List<Product> product = _context.Products.Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Skip((page-1)*12).Take(12).ToList();
            return View(product);
        }
        public IActionResult Details(int id)
        {
            Product product = _context.Products.Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).FirstOrDefault(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
