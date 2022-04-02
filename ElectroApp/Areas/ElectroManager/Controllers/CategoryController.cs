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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Categories.Count() / 6);
            List<Category> model = _context.Categories.Include(c=>c.ProductCategories).ThenInclude(pc=>pc.Product).Skip((page - 1) * 6).Take(6).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View();
            Category sname = _context.Categories.FirstOrDefault(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (sname != null)
            {
                ModelState.AddModelError("", "This category existed");
                return View();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            return View(category);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View();
            Category exCategory = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            Category checkName = _context.Categories.FirstOrDefault(c => c.Name == category.Name);
            if (checkName != null)
            {
                ModelState.AddModelError("Name", "This name is existed");
                return View(exCategory);
            }
            if (exCategory == null)
                return NotFound();

            Category sname = _context.Categories.FirstOrDefault(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (sname != null)
            {
                ModelState.AddModelError("", "This Category is existed");
                return View();
            }
            exCategory.Name = category.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return Json(new { status = 404 });
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Json(new {status=200});
        }
    }
}
