using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Http;
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

        public LayoutServices(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpcontext = httpContext;
        }
        public Setting getSettingsDatas()
        {
            Setting data = _context.Settings.FirstOrDefault();
            return data;
        }
        public List<Product> getProductDatas()
        {
            List<Product> prdata = _context.Products.Include(p=>p.Campaign).Include(p=>p.ProductComments).Include(p=>p.ProductImages).ToList();
            return prdata;
        }
        public BasketVM ShowBasket()
        {
            string basket = _httpcontext.HttpContext.Request.Cookies["Basket"];
            BasketVM basketData = new BasketVM
            {
                TotalPrice = 0,
                BasketItems = new List<BasketItemVM>(),
                Count = 0
            };
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
            return basketData;
        }
    }
}
