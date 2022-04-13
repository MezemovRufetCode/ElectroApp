using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Brands.Count() / 6);
           List<Brand> model = _context.Brands.Include(b=>b.Products).OrderByDescending(p => p.Id).Skip((page - 1) * 6).Take(6).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brand brand)
        {
            if (!ModelState.IsValid)
                return View();
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Brand brand = _context.Brands.FirstOrDefault(b => b.Id == id);
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Brand brand)
        {
            if (!ModelState.IsValid)
                return View();
            Brand existBrand = _context.Brands.FirstOrDefault(b => b.Id == brand.Id);
            Brand checkName = _context.Brands.FirstOrDefault(b => b.Name == brand.Name);
            if (checkName != null){
                ModelState.AddModelError("Name", "This brand name is existed");
                return View("existBrand");
            }
            if (existBrand == null)
            {
                return NotFound();
            }
            Brand sname = _context.Brands.FirstOrDefault(b => b.Name.ToLower().Trim() == brand.Name.ToLower().Trim());
            if (sname != null)
            {
                ModelState.AddModelError("", "This name is existed,try different one");
                return View();
            }
            existBrand.Name = brand.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            
        }
        public IActionResult Delete(int id)
        {
            Brand brand = _context.Brands.FirstOrDefault(b => b.Id == id);
            if(brand==null)
                return Json(new { status = 404 });
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
