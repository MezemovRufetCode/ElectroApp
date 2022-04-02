using ElectroApp.DAL;
using ElectroApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Services
{
    public class LayoutServices
    {
        private readonly AppDbContext _context;

        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }
        public Setting getSettingsDatas()
        {
            Setting data = _context.Settings.FirstOrDefault();
            return data;
        }
    }
}
