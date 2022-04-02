using ElectroApp.DAL;
using ElectroApp.Extentions;
using ElectroApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            Setting model = _context.Settings.FirstOrDefault();
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid)
                return View();

            Setting settings = _context.Settings.FirstOrDefault(s => s.Id == id);
            return View(settings);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Setting settings)
        {
            Setting exSett = _context.Settings.FirstOrDefault(s => s.Id == settings.Id);
            if (!ModelState.IsValid)
                return View();
            exSett.Logo = settings.Logo;
            exSett.hContact = settings.hContact;
            exSett.hEmail = settings.hEmail;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
