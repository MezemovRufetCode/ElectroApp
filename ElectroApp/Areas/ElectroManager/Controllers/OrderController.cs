using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _usermanager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _usermanager = userManager;
        }
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult Edit(int id)
        {
            Order order = _context.Orders.Include(o => o.OrderItems).ThenInclude(o => o.Product).ThenInclude(p => p.ProductImages).Include(o => o.AppUser).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }
        public IActionResult Accept(int id, string message)
        {
            Order order = _context.Orders.Include(o => o.AppUser).FirstOrDefault(o => o.Id == id);
            if (order == null) return Json(new { status = 400 });
            order.Status = true;
            if (message != null)
            {
                order.Message = message;
            }
            else
            {
                message = "Order accepted";
            }

            _context.SaveChanges();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mezemovrufet2020@gmail.com", "Electro");
            mail.To.Add(new MailAddress(order.AppUser.Email));
            mail.Subject = "Order accepted";
            mail.Body = $"{message}";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("mezemovrufetcode@gmail.com", "Mezemov15032000Code");
            smtp.Send(mail);
            return Json(new { status = 200 });
        }
        public IActionResult Reject(int id, string message)
        {
            Order order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return Json(new { status = 400 });
            order.Status = false;
            order.Message = message;
            _context.SaveChanges();
            if (message != null)
            {
                order.Message = message;
            }
            else
            {
                message = "Order rejected";
            }
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mezemovrufet2020@gmail.com", "Electro");
            mail.To.Add(new MailAddress(order.AppUser.Email));
            mail.Subject = "Order declined";
            mail.Body = $"{message}";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("mezemovrufetcode@gmail.com", "Mezemov15032000Code");
            smtp.Send(mail);
            return Json(new { status = 200 });
        }
    }
}
