using ElectroApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<bTag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<IntroSlider> IntroSliders { get; set; }
    }
}
