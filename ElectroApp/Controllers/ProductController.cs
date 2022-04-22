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
        public IActionResult Index(int? brandId, int? categoryId, int filterId, int page = 1)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 8);
            ProductVM productVM = new ProductVM
            {
                Products = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
             ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
             Include(p => p.Features).Include(p => p.Specs).OrderBy(p => p.Price).ToList(),
                Brands = _context.Brands.Include(b => b.Products).ThenInclude(p => p.Brand).ToList(),
                Categories = _context.Categories.Include(c => c.ProductCategories).ThenInclude(pc => pc.Product).ToList(),
                productCategory = _context.ProductCategories.Include(pc => pc.Product).Include(pc => pc.Category).FirstOrDefault(),
            };
            switch (filterId)
            {
                case 1:
                    productVM.Products = _context.Products.OrderBy(p => p.Name).ToList();
                    break;

                case 2:
                    productVM.Products = _context.Products.OrderByDescending(p => p.Name).ToList();
                    break;

                case 3:
                    productVM.Products = _context.Products.OrderBy(p => p.Price).ToList();
                    break;

                case 4:
                    productVM.Products = _context.Products.OrderByDescending(p => p.Price).ToList();
                    break;

                case 5:
                    productVM.Products = _context.Products.OrderBy(p => p.Id).ToList();
                    break;

                case 6:
                    productVM.Products = _context.Products.OrderByDescending(p => p.Id).ToList();
                    break;
                case 7:
                    productVM.Products = _context.Products.Where(p => p.Price <= 100).ToList();
                    break;
                case 8:
                    productVM.Products = _context.Products.Where(p => p.Price > 100 && p.Price <= 300).ToList();
                    break;
                case 9:
                    productVM.Products = _context.Products.Where(p => p.Price > 300).ToList();
                    break;
                default:
                    break;
            }
            //switch (priceId)
            //{
            //    case 1:
            //        productVM.Products = _context.Products.Where(p => p.Price<=100).ToList();
            //        break;

            //    case 2:
            //        productVM.Products = _context.Products.Where(p => p.Price>100 && p.Price<=300).ToList();
            //        break;

            //    case 3:
            //        productVM.Products = _context.Products.Where(p => p.Price>300).ToList();
            //        break;
            //    default:
            //        productVM.Products = _context.Products.ToList();
            //        break;
            //}
            //productVM.Products = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
            // ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
            // Include(p => p.Features).Include(p => p.Specs).ToList();




            if (brandId != null)
            {
                productVM.Products = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
               ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
               Include(p => p.Features).Include(p => p.Specs).Where(p => p.BrandId == brandId).ToList();
            }
            if (categoryId != null)
            {
                productVM.Products = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
               ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
               Include(p => p.Features).Include(p => p.Specs).Where(p => p.ProductCategories.Any(p => p.CategoryId == categoryId)).ToList();
            }
            return View(productVM);
        }
        public IActionResult Details(int id, int categoryId)
        {
            ViewBag.RelatedProducts = _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
                Include(p => p.ProductImages).Include(p => p.Campaign).Include(p => p.Brand).Where(p => p.ProductCategories.Any(p => p.CategoryId == categoryId && p.Id != id))
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
                return PartialView("_basketPartialView");
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
                    return PartialView("_basketPartialView");
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
                    return PartialView("_basketPartialView");
                }
            }

            //return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> DeleteBasketItem(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
                List<BasketItem> basketItems = _context.BasketItems.Where(b => b.ProductId == id && b.AppUserId == user.Id).ToList();
                foreach (var item in basketItems)
                {
                    _context.BasketItems.Remove(item);
                }
            }
            else
            {
                string basket = HttpContext.Request.Cookies["Basket"];
                List<BasketCookieItemVM> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);
                BasketCookieItemVM cookieItem = basketCookieItems.FirstOrDefault(c => c.Id == id);
                basketCookieItems.Remove(cookieItem);
                string basketStr = JsonConvert.SerializeObject(basketCookieItems);
                HttpContext.Response.Cookies.Append("Basket", basketStr);
            }
            _context.SaveChanges();
            return PartialView("_basketPartialView");
        }
        public IActionResult GetPartial()
        {
            return PartialView("_basketPartialView");
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
        public async Task<IActionResult> AddWishlist(int id)
        {
            Product product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);

                WishlistItem wishlistItem = _context.WishlistItems.FirstOrDefault(b => b.ProductId == product.Id && b.AppUserId == user.Id);
                if (wishlistItem == null)
                {
                    wishlistItem = new WishlistItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Count = 1
                    };
                    _context.WishlistItems.Add(wishlistItem);
                }
                else
                {
                    wishlistItem.Count++;
                }
                _context.SaveChanges();
                return PartialView("_wishlistPartialView");
            }
            else
            {
                string wishlist = HttpContext.Request.Cookies["Wishlist"];
                if (wishlist == null)
                {
                    List<WishlistCookieItemVM> wishlistCookieItems = new List<WishlistCookieItemVM>();
                    wishlistCookieItems.Add(new WishlistCookieItemVM
                    {
                        Id = product.Id,
                        Count = 1
                    });
                    string wishlistStr = JsonConvert.SerializeObject(wishlistCookieItems);
                    HttpContext.Response.Cookies.Append("Wishlist", wishlistStr);
                    return PartialView("_wishlistPartialView");
                }
                else
                {
                    List<WishlistCookieItemVM> wishlistCookieItems = JsonConvert.DeserializeObject<List<WishlistCookieItemVM>>(wishlist);
                    WishlistCookieItemVM cookiewItem = wishlistCookieItems.FirstOrDefault(c => c.Id == product.Id);
                    if (cookiewItem == null)
                    {
                        cookiewItem = new WishlistCookieItemVM
                        {
                            Id = product.Id,
                            Count = 1
                        };
                        wishlistCookieItems.Add(cookiewItem);
                    }
                    else
                    {
                        cookiewItem.Count++;
                    }
                    string wishlistStr = JsonConvert.SerializeObject(wishlistCookieItems);
                    HttpContext.Response.Cookies.Append("Wishlist", wishlistStr);
                    return PartialView("_wishlistPartialView");
                }
            }
        }
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
                List<WishlistItem> wishlistItems = _context.WishlistItems.Where(b => b.ProductId == id && b.AppUserId == user.Id).ToList();
                foreach (var item in wishlistItems)
                {
                    _context.WishlistItems.Remove(item);
                }
            }
            else
            {
                string wishlist = HttpContext.Request.Cookies["Wishlist"];
                List<WishlistCookieItemVM> wishlistCookieItems = JsonConvert.DeserializeObject<List<WishlistCookieItemVM>>(wishlist);
                WishlistCookieItemVM cookiwItem = wishlistCookieItems.FirstOrDefault(c => c.Id == id);
                wishlistCookieItems.Remove(cookiwItem);
                string wishlistStr = JsonConvert.SerializeObject(wishlistCookieItems);
                HttpContext.Response.Cookies.Append("Wishlist", wishlistStr);
            }
            _context.SaveChanges();
            return PartialView("_wishlistPartialView");
        }
        public IActionResult ShowWishlist()
        {
            string wishlistStr = HttpContext.Request.Cookies["Wishlist"];
            if (!string.IsNullOrEmpty(wishlistStr))
            {
                List<WishlistCookieItemVM> wishlist = JsonConvert.DeserializeObject<List<WishlistCookieItemVM>>(wishlistStr);
                return Json(wishlist);
            }
            return Content("Wishlist is empty");
        }
        public IActionResult GetWishPartial()
        {
            return PartialView("_wishlistPartialView");
        }


        public IActionResult AddCompare(int id)
        {
            Product product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).Include(p=>p.Brand).Include(p=>p.Specs).FirstOrDefault(p => p.Id == id);
            string compare = HttpContext.Request.Cookies["Compare"];
            if (compare == null)
            {
                List<CompareCookieItemVM> compareCookieItems = new List<CompareCookieItemVM>();
                compareCookieItems.Add(new CompareCookieItemVM
                {
                    Id = product.Id,
                    Count = 1
                });
                string compareStr = JsonConvert.SerializeObject(compareCookieItems);
                HttpContext.Response.Cookies.Append("Compare", compareStr);
                return PartialView("_comparePartialView");
            }
            else
            {
                List<CompareCookieItemVM> compareCookieItems = JsonConvert.DeserializeObject<List<CompareCookieItemVM>>(compare);
                CompareCookieItemVM cookiecItem = compareCookieItems.FirstOrDefault(c => c.Id == product.Id);
                if (cookiecItem == null)
                {
                    cookiecItem = new CompareCookieItemVM
                    {
                        Id = product.Id,
                        Count = 1
                    };
                    compareCookieItems.Add(cookiecItem);
                }
                else
                {
                    cookiecItem.Count++;
                }
                string compareStr = JsonConvert.SerializeObject(compareCookieItems);
                HttpContext.Response.Cookies.Append("Compare", compareStr);
                return PartialView("_comparePartialView");
            }

        }
        public IActionResult DeleteCompareItem(int id)
        {
            string compare = HttpContext.Request.Cookies["Compare"];
            List<CompareCookieItemVM> compareCookieItems = JsonConvert.DeserializeObject<List<CompareCookieItemVM>>(compare);
            CompareCookieItemVM cookiecItem = compareCookieItems.FirstOrDefault(c => c.Id == id);
            compareCookieItems.Remove(cookiecItem);
            string compareStr = JsonConvert.SerializeObject(compareCookieItems);
            HttpContext.Response.Cookies.Append("Compare", compareStr);
            _context.SaveChanges();
            return PartialView("_comparePartialView");
        }
        public IActionResult ShowCompare()
        {
            string compareStr = HttpContext.Request.Cookies["Compare"];
            if (!string.IsNullOrEmpty(compareStr))
            {
                List<CompareCookieItemVM> compare = JsonConvert.DeserializeObject<List<CompareCookieItemVM>>(compareStr);
                return Json(compare);
            }
            return Content("Compare is empty");
        }
        public IActionResult GetComparePartial()
        {
            return PartialView("_comparePartialView");
        }
        public IActionResult SearchResult(string search, int? categoryId)
        {
            ViewBag.Categories = _context.Categories.ToList();
            //List<Product> products = _context.Products.Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim())).ToList();
            ProductVM productVM = new ProductVM
            {
                Products = search == null ? _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
                Include(p => p.Features).Include(p => p.Specs).ToList() : _context.Products.Include(p => p.ProductComments).ThenInclude(p => p.AppUser).Include(p => p.Brand).Include(p => p.ProductCategories).
                ThenInclude(pc => pc.Category).Include(p => p.ProductImages).Include(p => p.Campaign).
                Include(p => p.Features).Include(p => p.Specs).Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim())).ToList(),
                Categories = _context.Categories.Include(c => c.ProductCategories).ThenInclude(pc => pc.Product).ToList(),
                Brands = _context.Brands.Include(b => b.Products).ThenInclude(p => p.Brand).ToList()
            };
            return View(productVM);
        }
    }
}
