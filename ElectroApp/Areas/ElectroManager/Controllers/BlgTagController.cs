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
    public class BlgTagController : Controller
    {
        private readonly AppDbContext _context;
        public BlgTagController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Tags.Count() / 6);
            List<bTag> model = _context.Tags.Include(t => t.BlogTags).ThenInclude(bt => bt.Blog).Skip((page-1)*6).Take(6).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(bTag tag)
        {
            if (!ModelState.IsValid)
                return View();
            bTag sameTag = _context.Tags.FirstOrDefault(t => t.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
            if(sameTag != null)
            {
                ModelState.AddModelError("", "This name existed,try different");
                return View();
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            bTag tag = _context.Tags.FirstOrDefault(t => t.Id == id);
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(bTag tag)
        {
            if (!ModelState.IsValid)
                return View();
            bTag existTag = _context.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (existTag == null)
            {
                return NotFound();
            }
            bTag checkName = _context.Tags.FirstOrDefault(t => t.Name.ToLower().Trim() == tag.Name.ToLower().Trim());
            if (checkName != null)
            {
                ModelState.AddModelError("", "This name is existed,try different one");
                return View();
            }
            existTag.Name = tag.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            bTag tag = _context.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null)
                return Json(new { status = 404 });
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
