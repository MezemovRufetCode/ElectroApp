using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly AppDbContext _context;

        public OrderController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _usermanager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Checkout()
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new OrderVM
            {
                Fullname = user.Fullname,
                Username = user.UserName,
                Email = user.Email,
                BasketItems = _context.BasketItems.Include(b => b.Product).ThenInclude(p => p.Campaign).
                Include(b => b.Product).ThenInclude(p => p.ProductImages).Where(b => b.AppUserId == user.Id).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            OrderVM model = new OrderVM
            {
                Fullname = user.Fullname,
                Username = user.UserName,
                Email = user.Email,
                BasketItems = _context.BasketItems.Include(b => b.Product).ThenInclude(p => p.Campaign).
                Include(b => b.Product).ThenInclude(p => p.ProductImages).Where(b => b.AppUserId == user.Id).ToList()
            };
            if (!ModelState.IsValid)
                return View(model);
            TempData["Succeded"] = false;
            if (model.BasketItems.Count == 0)
            {
                ModelState.AddModelError("", "Basket is empty");
                return View(model);
            }
            
            Order order = new Order
            {
                Country = orderVM.Country,
                City = orderVM.City,
                Adress = orderVM.Adress,
                AdressSec = orderVM.AdressSec,
                TotalPrice = 0,
                OrderDate = DateTime.Now,
                AppUserId = user.Id
            };
            foreach (BasketItem item in model.BasketItems)
            {
                order.TotalPrice += item.Product.CampaignId == null ? item.Count * item.Product.Price : item.Count * item.Product.Price * (100 - item.Product.Campaign.DiscountPercent) / 100;
                OrderItem orderItem = new OrderItem
                {
                    Name = item.Product.Name,
                    Price = item.Count * (item.Product.CampaignId == null ? item.Product.Price : item.Product.Price * (100 - item.Product.Campaign.DiscountPercent) / 100),
                    AppUserId = user.Id,
                    ProductId = item.Product.Id,
                    Order = order,
                    Count = item.Count
                };
                _context.OrderItems.Add(orderItem);
            }
            _context.BasketItems.RemoveRange(model.BasketItems);
            _context.Orders.Add(order);
            TempData["Succeded"] = true;
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
