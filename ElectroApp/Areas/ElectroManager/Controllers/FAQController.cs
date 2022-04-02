using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class FAQController : Controller
    {
        private readonly AppDbContext _context;

        public FAQController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.FAQs.Count() / 6);
            List<FAQ> model = _context.FAQs.Skip((page-1)*6).Take(6).ToList();
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FAQ faq)
        {
            if (!ModelState.IsValid)
                return View();
            _context.FAQs.Add(faq);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            FAQ faq = _context.FAQs.FirstOrDefault(f => f.Id == id);
            return View(faq);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FAQ faq)
        {
            FAQ exFaq = _context.FAQs.FirstOrDefault(f => f.Id == faq.Id);
            if (exFaq == null)
                return NotFound();
            exFaq.Answer = faq.Answer;
            exFaq.Question = faq.Question;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            FAQ faq = _context.FAQs.FirstOrDefault(f => f.Id == id);
            if (faq == null)
                return Json(new { status = 404 });
            _context.FAQs.Remove(faq);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
