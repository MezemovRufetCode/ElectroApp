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
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 8);
            List<Product> product = _context.Products.Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p=>p.Campaign).Skip((page-1)*8).Take(8).ToList();
            return View(product);
        }
        public IActionResult Details(int id)
        {
            ViewBag.Categories = _context.Categories.ToList();
            Product product = _context.Products.Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).ThenInclude(pc=>pc.Image).Include(p => p.Campaign).FirstOrDefault(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
