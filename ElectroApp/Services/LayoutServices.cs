using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Services
{
    public class LayoutServices
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpcontext;
        private readonly UserManager<AppUser> _usermanager;

        public LayoutServices(AppDbContext context, IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpcontext = httpContext;
            _usermanager = userManager;
        }
        public Setting getSettingsDatas()
        {
            Setting data = _context.Settings.FirstOrDefault();
            return data;
        }
        public List<Product> getProductDatas()
        {
            List<Product> prdata = _context.Products.Include(p => p.Campaign).Include(p => p.ProductComments).Include(p => p.ProductImages).ToList();
            return prdata;
        }
        public async Task<BasketVM> ShowBasket()
        {
            string basket = _httpcontext.HttpContext.Request.Cookies["Basket"];
            BasketVM basketData = new BasketVM
            {
                TotalPrice = 0,
                BasketItems = new List<BasketItemVM>(),
                Count = 0
            };
            if (_httpcontext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.FindByNameAsync(_httpcontext.HttpContext.User.Identity.Name);
                List<BasketItem> basketItems = _context.BasketItems.Include(b => b.AppUser).Where(b => b.AppUserId == user.Id).ToList();
                foreach (BasketItem item in basketItems)
                {
                    Product product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        BasketItemVM basketItemVM = new BasketItemVM
                        {
                            Product = product,
                            Count = item.Count
                        };
                        basketItemVM.Price = product.CampaignId == null ? product.Price : product.Price * (100 - product.Campaign.DiscountPercent) / 100;
                        basketData.BasketItems.Add(basketItemVM);
                        basketData.Count++;
                        basketData.TotalPrice += basketItemVM.Price * basketItemVM.Count;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(basket))
                {
                    List<BasketCookieItemVM> basketCookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);
                    foreach (BasketCookieItemVM item in basketCookieItems)
                    {
                        Product product = _context.Products.FirstOrDefault(p => p.Id == item.Id);
                        if (product != null)
                        {
                            BasketItemVM basketItem = new BasketItemVM
                            {
                                Product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == item.Id),
                                Count = item.Count,
                            };
                            basketItem.Price = basketItem.Product.CampaignId == null ? basketItem.Product.Price : basketItem.Product.Price * (100 - basketItem.Product.Campaign.DiscountPercent) / 100;
                            basketData.BasketItems.Add(basketItem);
                            basketData.Count++;
                            basketData.TotalPrice += basketItem.Price * basketItem.Count;
                        }
                    }
                }
            }

            return basketData;
        }

        public async Task<WishlistVM> ShowWishlist()
        {
            string wishlist = _httpcontext.HttpContext.Request.Cookies["Wishlist"];
            WishlistVM wishlistData = new WishlistVM
            {
                TotalPrice = 0,
                WishlistItems = new List<WishlistItemVM>(),
                Count = 0
            };
            if (_httpcontext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser user = await _usermanager.FindByNameAsync(_httpcontext.HttpContext.User.Identity.Name);
                List<WishlistItem> wishlistItems = _context.WishlistItems.Include(b => b.AppUser).Where(b => b.AppUserId == user.Id).ToList();
                foreach (WishlistItem item in wishlistItems)
                {
                    Product product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        WishlistItemVM wishlistItemVM = new WishlistItemVM
                        {
                            Product = product,
                            Count = item.Count
                        };
                        wishlistItemVM.Price = product.CampaignId == null ? product.Price : product.Price * (100 - product.Campaign.DiscountPercent) / 100;
                        wishlistData.WishlistItems.Add(wishlistItemVM);
                        wishlistData.Count++;
                        wishlistData.TotalPrice += wishlistItemVM.Price * wishlistItemVM.Count;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(wishlist))
                {
                    List<WishlistCookieItemVM> wishlistCookieItems = JsonConvert.DeserializeObject<List<WishlistCookieItemVM>>(wishlist);
                    foreach (WishlistCookieItemVM item in wishlistCookieItems)
                    {
                        Product product = _context.Products.FirstOrDefault(p => p.Id == item.Id);
                        if (product != null)
                        {
                            WishlistItemVM wishlistItem = new WishlistItemVM
                            {
                                Product = _context.Products.Include(p => p.Campaign).Include(p => p.ProductImages).FirstOrDefault(p => p.Id == item.Id),
                                Count = item.Count,
                            };
                            wishlistItem.Price = wishlistItem.Product.CampaignId == null ? wishlistItem.Product.Price : wishlistItem.Product.Price * (100 - wishlistItem.Product.Campaign.DiscountPercent) / 100;
                            wishlistData.WishlistItems.Add(wishlistItem);
                            wishlistData.Count++;
                            wishlistData.TotalPrice += wishlistItem.Price * wishlistItem.Count;
                        }
                    }
                }
            }
            return wishlistData;
        }
    }
}
