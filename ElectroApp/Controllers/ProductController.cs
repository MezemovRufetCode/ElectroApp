using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _usermanager;

        public ProductController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _usermanager = userManager;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 8);
            List<Product> product = _context.Products.Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p=>p.Campaign).Include(p=>p.Features).Include(p=>p).Skip((page-1)*8).Take(8).ToList();
            return View(product);
        }
        public IActionResult Details(int id)
        {
            ViewBag.Categories = _context.Categories.ToList();
            Product product = _context.Products.Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).Include(p=>p.ProductComments).Include(p=>p.Specs).Include(p=>p.Features).FirstOrDefault(p=>p.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddComment(ProductComment comment)
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Product", new { id = comment.ProductId });
            if (!_context.Products.Any(p => p.Id == comment.ProductId))
                return NotFound();
            ProductComment productComment = new ProductComment()
            {
                Title = comment.Title,
                ProductId=comment.ProductId,
                Text=comment.Text,
                WriteDate=DateTime.Now,
                AppUserId=comment.AppUserId
            };
            _context.ProductComments.Add(productComment);
            _context.SaveChanges();
            return RedirectToAction("Details", "Products", new { id = comment.ProductId });
        }
       [Authorize]
       public async Task<IActionResult> DeleteComment(int id)
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Product");
            if (!_context.ProductComments.Any(c => c.Id == id && c.AppUserId == user.Id))
                return NotFound();
            ProductComment comment = _context.ProductComments.FirstOrDefault(c => c.Id == id && c.AppUserId == user.Id);
            _context.ProductComments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("Details", "Product", new { id = comment.ProductId });
        }
    }
}
