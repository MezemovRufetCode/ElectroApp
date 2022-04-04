using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    public class DashBoard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
