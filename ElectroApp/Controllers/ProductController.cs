using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _usermanager = userManager;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 8);
            ProductVM productVM = new ProductVM
            {
                Products = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
                Include(p => p.Features).Include(p => p.Specs).ToList(),
                Brands = _context.Brands.Include(b => b.Products).ThenInclude(p => p.Brand).ToList()
            };
            return View(productVM);
        }
        public IActionResult Details(int id, int categoryId)
        {
            ViewBag.RelatedProducts = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
                Include(p => p.ProductImages).Include(p => p.Campaign).Include(p => p.Brand).Where(p => p.ProductCategories.FirstOrDefault().CategoryId == categoryId && p.Id != id)
                .OrderByDescending(p => p.Id).Take(6).ToList();
            ViewBag.Categories = _context.Categories.ToList();
            Product product = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).Include(p => p.Specs).Include(p => p.Features).FirstOrDefault(p => p.Id == id);
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
                ProductId = comment.ProductId,
                Text = comment.Text,
                WriteDate = DateTime.Now,
                AppUserId = user.Id
            };
            _context.ProductComments.Add(productComment);
            _context.SaveChanges();
            return RedirectToAction("Details", "Product", new { id = comment.ProductId });
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
        public async Task<IActionResult> AddBasket(int id)
        {
            Product product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);

                BasketItem basketItem = _context.BasketItems.FirstOrDefault(b => b.ProductId == product.Id && b.AppUserId == user.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                _context.SaveChanges();
            }
            else
            {
                string basket = HttpContext.Request.Cookies["Basket"];
                if (basket == null)
                {
                    List<BasketCookieItemVM> basketCookieItems = new List<BasketCookieItemVM>();
                    basketCookieItems.Add(new BasketCookieItemVM
                    {
                        Id = product.Id,
                        Count = 1
                    });
                    string basketStr = JsonConvert.SerializeObject(basketCookieItems);
                    HttpContext.Response.Cookies.Append("Basket", basketStr);
                }
                else
                {
                    List<BasketCookieItemVM> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);
                    BasketCookieItemVM cookieItem = basketCookieItems.FirstOrDefault(c => c.Id == product.Id);
                    if (cookieItem == null)
                    {
                        cookieItem = new BasketCookieItemVM
                        {
                            Id = product.Id,
                            Count = 1
                        };
                        basketCookieItems.Add(cookieItem);
                    }
                    else
                    {
                        cookieItem.Count++;
                    }
                    string basketStr = JsonConvert.SerializeObject(basketCookieItems);
                    HttpContext.Response.Cookies.Append("Basket", basketStr);
                }
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ShowBasket()
        {
            string basketStr = HttpContext.Request.Cookies["Basket"];
            if (!string.IsNullOrEmpty(basketStr))
            {
                List<BasketCookieItemVM> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);
                return Json(basket);
            }
            return Content("Basket is empty");
        }
        public IActionResult SearchResult(string search)
        {
            //List<Product> products = _context.Products.Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim())).ToList();
            ProductVM productVM = new ProductVM
            {
                Products = search == null ? _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
                Include(p => p.Features).Include(p => p.Specs).ToList() : _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
                Include(p => p.Features).Include(p => p.Specs).Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim())).ToList(),
                Brands = _context.Brands.Include(b => b.Products).ThenInclude(p => p.Brand).ToList()
            };
            return View(productVM);
        }
    }
}
