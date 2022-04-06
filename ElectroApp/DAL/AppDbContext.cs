using ElectroApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.DAL
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<bTag> Tags { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<IntroSlider> IntroSliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<Specs> Specs { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<FAQ> FAQs { get; set; }

    }
}
