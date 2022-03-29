using ElectroApp.DAL;
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
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Blogs.Count() / 6);
            List<Blog> model = _context.Blogs.Include(b=>b.BlogTags).ThenInclude(bt=>bt.Tag).Skip((page - 1) * 6).Take(6).ToList();
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog blog)
        {
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
                return View();
            blog.BlogTags = new List<BlogTag>();
            foreach (int id in blog.TagIds)
            {
                BlogTag bTag = new BlogTag
                {
                    TagId=id,
                    Blog=blog
                };
                blog.BlogTags.Add(bTag);
            }

            if (blog.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Include Image");
                return View();
            }
            if (!blog.ImageFile.CheckSize(3))
            {
                ModelState.AddModelError("ImageFile", "Max size can be 3 Mb");
                return View();
            }
            if (!blog.ImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Please Choose image file");
                return View();
            }
            blog.Image = blog.ImageFile.SaveImg(_env.WebRootPath, "assets/images/blog");

            //Blog blg = new Blog
            //{
            //    Title = blog.Title,
            //    Desc = blog.Desc,
            //    PublishDate = DateTime.Now,
            //    Image = blog.Image,
            //    WrittenBy = "Admin"
            //};
            blog.PublishDate = DateTime.Now;
            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            if (!ModelState.IsValid)
                return View();
            ViewBag.Tags = _context.Tags.ToList();
            Blog blog = _context.Blogs.Include(b=>b.BlogTags).ThenInclude(bt=>bt.Blog).FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Blog blog)
        {
            ViewBag.Tags = _context.Tags.ToList();
            Blog existBlog = _context.Blogs.Include(b=>b.BlogTags).ThenInclude(bt=>bt.Tag).FirstOrDefault(b => b.Id == blog.Id);
            if (!ModelState.IsValid)
                return View(existBlog);
            if(blog.ImageFile != null)
            {
                if (!blog.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Please select image file");
                    return View(existBlog);
                }
                if (!blog.ImageFile.CheckSize(3))
                {
                    ModelState.AddModelError("ImageFile", "Image size max can be 3 Mb");
                    return View(existBlog);
                }
                Helpers.Helper.DeleteImg(_env.WebRootPath, "assets/images/blog",existBlog.Image);
            }


            List<BlogTag> removableTag = existBlog.BlogTags.Where(fc => !blog.TagIds.Contains(fc.Id)).ToList();
            existBlog.BlogTags.RemoveAll(fc => removableTag.Any(rc => fc.Id == rc.Id));
            foreach (var tagId in blog.TagIds)
            {
                BlogTag blogTag = existBlog.BlogTags.FirstOrDefault(fc => fc.TagId == tagId);
                if (blogTag == null)
                {
                    BlogTag btag = new BlogTag
                    {
                        TagId = tagId,
                        BlogId = existBlog.Id
                    };
                    existBlog.BlogTags.Add(btag);
                }
            }


            existBlog.Title = blog.Title;
            existBlog.Desc = blog.Desc;
            existBlog.WrittenBy = blog.WrittenBy;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Blog blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
                return Json(new { status = 404 });
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
