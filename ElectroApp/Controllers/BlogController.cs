using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.LatestBlogs = _context.Blogs.OrderBy(b => b.PublishDate).Take(3).ToList();
            List<Blog> model = _context.Blogs.ToList();
            return View(model);
        }
        public IActionResult Details(int id)
        {
            ViewBag.LatestBlogs = _context.Blogs.OrderBy(b => b.PublishDate).Take(3).ToList();
            Blog blog = _context.Blogs.Include(b=>b.BlogTags).ThenInclude(bt=>bt.Tag).FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
    }
}
