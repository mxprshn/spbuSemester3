using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestRunnerRepository testRunnerRepository;
        private readonly IWebHostEnvironment environment;

        public HomeController(TestRunnerRepository testRunnerRepository, IWebHostEnvironment environment)
        {
            this.testRunnerRepository = testRunnerRepository;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("TestRunner", testRunnerRepository.AssemblyFileModels.ToList());
        }

        [HttpPost]
        public IActionResult AddAssembly(IFormFile file)
        {
            if (file != null)
            {
                using (var fileStream = new FileStream($"{environment.WebRootPath}/Temp/{file.FileName}", FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                testRunnerRepository.Add(new AssemblyModel { Name = file.FileName });
                testRunnerRepository.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        public IActionResult History()
        {
            return View();
        }
    }
}