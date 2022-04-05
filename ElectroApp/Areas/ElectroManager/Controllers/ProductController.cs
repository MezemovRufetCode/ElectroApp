﻿using ElectroApp.DAL;
using ElectroApp.Extentions;
using ElectroApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Products.Count() / 4);
            List<Product> model = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
                Include(p => p.ProductImages).Include(p => p.Brand).Include(p=>p.Campaign).
                Skip((page - 1) * 4).Take(4).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Campaigns = _context.Campaigns.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            ViewBag.Campaigns = _context.Campaigns.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            if (!ModelState.IsValid)
                return View();

            if (product.CampaignId == 0)
            {
                product.CampaignId = null;
            }

            product.ProductCategories = new List<ProductCategory>();
            product.ProductImages = new List<ProductImage>();
            foreach (int id in product.CategoryIds)
            {
                ProductCategory prCat = new ProductCategory
                {
                    Product = product,
                    CategoryId = id,

                };
                product.ProductCategories.Add(prCat);
            }

            if (product.ImageFiles.Count > 6)
            {
                ModelState.AddModelError("ImageFiles", "You can choose only 6 images");
                return View();
            }
            foreach (var image in product.ImageFiles)
            {
                if (!image.IsImage())
                {
                    ModelState.AddModelError("ImageFiles", "You can include only image files");
                    return View();
                }
                if (!image.CheckSize(3))
                {
                    ModelState.AddModelError("ImageFiles", "Image size max can be 3 mb");
                    return View();
                }
            }
            foreach (var image in product.ImageFiles)
            {
                ProductImage productImage = new ProductImage
                {
                    Image = image.SaveImg(_env.WebRootPath, "assets/images/featuredProducts"),
                    IsMain = product.ProductImages.Count < 1 ? true : false,
                    Product = product
                };
                product.ProductImages.Add(productImage);
            }
            if (product.Price < product.CostPrice)
            {
                ModelState.AddModelError("CostPrice", "Cost Price can not be higher than sale price");
                return View();
            }
                
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Edit(int id)
        {
            ViewBag.Campaigns = _context.Campaigns.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            Product product = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
                Include(p => p.ProductImages).Include(p => p.Brand).Include(p=>p.Campaign).FirstOrDefault(p=>p.Id==id);
            if (product == null)
                return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            ViewBag.Campaigns = _context.Campaigns.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Brands = _context.Brands.ToList();
            Product existProduct = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
                Include(p => p.ProductImages).Include(p => p.Brand).Include(p=>p.Campaign).FirstOrDefault(p => p.Id == product.Id);
            if (!ModelState.IsValid)
                return View(existProduct);
            if (existProduct == null)
                return NotFound();

            //Product checkName =_context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).
            //    Include(p => p.ProductImages).Include(p => p.Brand).FirstOrDefault(p => p.Name.ToLower().Trim() == product.Name.ToLower().Trim());

            //if (checkName != null)
            //{
            //    ModelState.AddModelError("Name", "This name existed,try different name");
            //    return View(existProduct);
            //}

            if (product.ImageFiles != null)
            {
                foreach (var image in product.ImageFiles)
                {
                    if (!image.IsImage())
                    {
                        ModelState.AddModelError("ImageFiles", "Please select image file");
                        return View(existProduct);
                    }
                    if (!image.CheckSize(3))
                    {
                        ModelState.AddModelError("ImageFiles", "Image size max can be 3Mb");
                        return View(existProduct);
                    }
                }
                List<ProductImage> removableImages = existProduct.ProductImages.Where(pi => pi.IsMain == false && !product.ImageIds.Contains(pi.Id)).ToList();
                existProduct.ProductImages.RemoveAll(pi => removableImages.Any(ri => ri.Id == pi.Id));

                foreach (var item in removableImages)
                {
                    Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/featuredProducts", item.Image);
                }
                foreach (var image in product.ImageFiles)
                {
                    ProductImage productImage = new ProductImage
                    {
                        Image = image.SaveImg(_env.WebRootPath, "assets/images/featuredProducts"),
                        IsMain = false,
                        ProductId = existProduct.Id
                    };
                    existProduct.ProductImages.Add(productImage);
                }
                List<ProductCategory> removableCategories = existProduct.ProductCategories.Where(pc => !product.CategoryIds.Contains(pc.Id)).ToList();
                existProduct.ProductCategories.RemoveAll(pc => removableCategories.Any(rc => pc.Id == rc.Id));
                foreach (var categoryId in product.CategoryIds)
                {
                    ProductCategory productCategory = existProduct.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
                    if (productCategory == null)
                    {
                        ProductCategory pCategory = new ProductCategory
                        {
                            CategoryId = categoryId,
                            ProductId = existProduct.Id
                        };
                        existProduct.ProductCategories.Add(pCategory);
                    }
                }
            }
            List<ProductCategory> removableCategories2 = existProduct.ProductCategories.Where(pc => !product.CategoryIds.Contains(pc.Id)).ToList();
            existProduct.ProductCategories.RemoveAll(pc => removableCategories2.Any(rc => pc.Id == rc.Id));
            foreach (var categoryId in product.CategoryIds)
            {
                ProductCategory productCategory = existProduct.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
                if (productCategory == null)
                {
                    ProductCategory pCategory = new ProductCategory
                    {
                        CategoryId = categoryId,
                        ProductId = existProduct.Id
                    };
                    existProduct.ProductCategories.Add(pCategory);
                }
            }
            existProduct.InStock = product.InStock;
            existProduct.SkuCode = product.SkuCode;
            existProduct.Price = product.Price;
            existProduct.CostPrice = product.CostPrice;
            existProduct.Name = product.Name;
            existProduct.Videolink = product.Videolink;
            existProduct.BrandId = product.BrandId;
            if (product.CampaignId == 0)
            {
                product.CampaignId = null;
            }
            existProduct.CampaignId = product.CampaignId;
            _context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Delete(int id)
        {
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return Json(new { status = 404 });
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
