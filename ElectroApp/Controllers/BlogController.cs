using ElectroApp.DAL;
using ElectroApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _usermanager;

        public BlogController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _usermanager = userManager;
        }
        public IActionResult Index()
        {
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.LatestBlogs = _context.Blogs.OrderBy(b => b.PublishDate).Take(3).ToList();
            List<Blog> model = _context.Blogs.Include(b=>b.Comments).ThenInclude(b=>b.AppUser).ToList();
            return View(model);
        }


        public IActionResult Details(int id)
        {
            ViewBag.Tags = _context.Tags.ToList();
            ViewBag.LatestBlogs = _context.Blogs.OrderBy(b => b.PublishDate).Take(3).ToList();
            Blog blog = _context.Blogs.Include(b=>b.Comments).ThenInclude(b=>b.AppUser).Include(b=>b.BlogTags).ThenInclude(bt=>bt.Tag).FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        //public IActionResult RelatedBlogs(int id)
        //{
        //    List<Blog> blogs = _context.Blogs.Include(b => b.BlogTags).ThenInclude(bt => bt.Tag).Where(b => b.TagIds.Contains(id)).ToList();
        //    if (blogs == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(blogs);
        //}
        [Authorize]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddComment(BlogComment comment)
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Blog", new { id = comment.BlogId });
            if (!_context.Blogs.Any(b => b.Id == comment.BlogId))
                return NotFound();
            BlogComment bcomment = new BlogComment
            {
                Text = comment.Text,
                BlogId = comment.BlogId,
                WriteTime = DateTime.Now,
                AppUserId = user.Id
            };
            _context.BlogComments.Add(bcomment);
            _context.SaveChanges();
            return RedirectToAction("Details", "Blog", new { id = comment.BlogId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid) return RedirectToAction("Details", "Blog");
            if (!_context.BlogComments.Any(c => c.Id == id && c.AppUserId == user.Id)) return NotFound();
            BlogComment bcomment = _context.BlogComments.FirstOrDefault(c => c.Id == id && c.AppUserId == user.Id);
            _context.BlogComments.Remove(bcomment);
            _context.SaveChanges();
            return RedirectToAction("Details", "Blog", new { id = bcomment.BlogId });
        }
        public IActionResult Search(string search)
        {
            List<Blog> blog = _context.Blogs.Include(b => b.Comments).ThenInclude(b => b.AppUser).Where(b => b.Title.ToLower().Trim().Contains(search.ToLower().Trim())).ToList();
            return PartialView("_BlogPartialView", blog);
        }
    }
}
