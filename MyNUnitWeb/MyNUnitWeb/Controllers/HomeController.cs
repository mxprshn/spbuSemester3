using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("TestRunner");
        }

        public IActionResult LoadAssembly(Assemblies ololo)
        {
            return View("TestRunner");
        }

        public IActionResult History()
        {
            return View();
        }
    }
}