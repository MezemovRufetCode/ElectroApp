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
    public class CampaignController : Controller
    {
        private readonly AppDbContext _context;

        public CampaignController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Campaign> model = _context.Campaigns.Include(c=>c.Products).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Campaign campaign)
        {
            if (!ModelState.IsValid)
                return View();
            //Campaign whatFor = _context.Campaigns.FirstOrDefault(c => c.WhatFor.ToLower().Trim() == campaign.WhatFor.ToLower().Trim());
            //if (whatFor != null)
            //{
            //    ModelState.AddModelError("","")
            //}
            Campaign checkCamp = _context.Campaigns.FirstOrDefault(c => (c.DiscountPercent == campaign.DiscountPercent) && (c.WhatFor.ToLower().Trim() == campaign.WhatFor.ToLower().Trim()));
            if (checkCamp != null)
            {
                ModelState.AddModelError("", "This campaign is existed");
                return View();
            }

            _context.Campaigns.Add(campaign);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            Campaign campaign = _context.Campaigns.FirstOrDefault(c => c.Id == id);
            return View(campaign);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Campaign campaign)
        {
            if (!ModelState.IsValid)
                return View();
            Campaign exCamp = _context.Campaigns.FirstOrDefault(c => c.Id == campaign.Id);
            if (exCamp == null)
                return NotFound();
            Campaign checkCamp = _context.Campaigns.FirstOrDefault(c => (c.DiscountPercent == campaign.DiscountPercent)&&(c.WhatFor.ToLower().Trim()==campaign.WhatFor.ToLower().Trim()));
            if (checkCamp != null)
            {
                ModelState.AddModelError("", "This campaign is existed");
                return View();
            }
            exCamp.WhatFor = campaign.WhatFor;
            exCamp.DiscountPercent = campaign.DiscountPercent;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Campaign campaign = _context.Campaigns.FirstOrDefault(c => c.Id == id);
            if (campaign == null)
                return Json(new { status = 404 });
            _context.Campaigns.Remove(campaign);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
