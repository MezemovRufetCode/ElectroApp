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
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<IntroSlider> model = _context.IntroSliders.ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IntroSlider slider)
        {
            if (!ModelState.IsValid)
                return View();
            if (slider.BackImageFile == null)
            {
                ModelState.AddModelError("BackImageFile", "Image filed can not be empty");
                return View();
            }
            if (!slider.BackImageFile.CheckSize(3))
            {
                ModelState.AddModelError("BackImageFile", "Banner image size max can be 3 Mb");
                return View();
            }
            if (!slider.BackImageFile.IsImage())
            {
                ModelState.AddModelError("BackImageFile", "You can include only image file");
                return View();
            }

            if (slider.BannerImgFile == null)
            {
                ModelState.AddModelError("BannerImgFile", "Banner Image field can not be empty");
                return View();
            }
            if (!slider.BannerImgFile.CheckSize(3))
            {
                ModelState.AddModelError("BannerImgFile", "Banner image size max can be 3 Mb");
                return View();
            }
            if (!slider.BannerImgFile.IsImage())
            {
                ModelState.AddModelError("BannerImgFile", "You can include only image file");
                return View();
            }


            slider.BackImage = slider.BackImageFile.SaveImg(_env.WebRootPath, "assets/images/slider");
            slider.SliderBannerImage = slider.BannerImgFile.SaveImg(_env.WebRootPath, "assets/images/slider");
            _context.Add(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            IntroSlider slider = _context.IntroSliders.FirstOrDefault(s => s.Id == id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IntroSlider slider)
        {
            IntroSlider existSlider = _context.IntroSliders.FirstOrDefault(s => s.Id == slider.Id);
            if (existSlider == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(existSlider);

            if (slider.BackImageFile != null && slider.BannerImgFile == null)
            {
                if (!slider.BackImageFile.CheckSize(3))
                {
                    ModelState.AddModelError("BackImageFile", "Image size max can be 3 Mb");
                    return View();
                }
                if (!slider.BackImageFile.IsImage())
                {
                    ModelState.AddModelError("BackImageFile", "U can not include file exist Image");
                    return View();
                }
                Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/slider", existSlider.BackImage);
                existSlider.BackImage = slider.BackImageFile.SaveImg(_env.WebRootPath, "assets/images/slider");
            }
            else
            if( slider.BannerImgFile!=null && slider.BackImageFile == null)
            {
                if (!slider.BannerImgFile.CheckSize(3))
                {
                    ModelState.AddModelError("BannerImgFile", "Image size max can be 3 Mb");
                    return View(existSlider);
                }
                if (!slider.BannerImgFile.IsImage())
                {
                    ModelState.AddModelError("BannerImgFile", "You can include only image file");
                    return View(existSlider);
                }
                if (existSlider.BannerImgFile != null)
                {
                    Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/slider", existSlider.SliderBannerImage);
                }
                existSlider.SliderBannerImage = slider.BannerImgFile.SaveImg(_env.WebRootPath, "assets/images/slider");
            }
            else 
            if(slider.BackImageFile!=null && slider.BannerImgFile != null)
            {
                if (!slider.BackImageFile.CheckSize(3))
                {
                    ModelState.AddModelError("BackImageFile", "Image size max can be 3 Mb");
                    return View(existSlider);
                }
                if (!slider.BackImageFile.IsImage())
                {
                    ModelState.AddModelError("BackImageFile", "You can include only image file ");
                    return View(existSlider);
                }

                if (!slider.BannerImgFile.CheckSize(3))
                {
                    ModelState.AddModelError("BannerImgFile", "Image size max can be 3 Mb");
                    return View(existSlider);
                }
                if (!slider.BannerImgFile.IsImage())
                {
                    ModelState.AddModelError("BannerImgFile", "You should include image file");
                    return View(existSlider);
                }


                if (existSlider.BannerImgFile != null)
                {
                    Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/slider", existSlider.SliderBannerImage);
                }

                existSlider.SliderBannerImage = slider.BannerImgFile.SaveImg(_env.WebRootPath, "assets/images/slider");

                Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/slider", existSlider.BackImage);
                existSlider.BackImage = slider.BackImageFile.SaveImg(_env.WebRootPath, "assets/images/slider");
            }
            existSlider.FirstDesc = slider.FirstDesc;
            existSlider.SecDesc = slider.SecDesc;
            existSlider.Price = slider.Price;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            IntroSlider slider = _context.IntroSliders.FirstOrDefault(s => s.Id == id);
            if (slider == null)
                return Json(new { status = 404 });
            _context.IntroSliders.Remove(slider);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
