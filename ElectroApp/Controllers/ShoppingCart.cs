using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class ShoppingCart : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
