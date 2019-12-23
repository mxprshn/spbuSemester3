using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestArchive testArchive;
        private readonly IWebHostEnvironment environment;
        private CurrentStateModel currentState;

        public HomeController(TestArchive testArchive, IWebHostEnvironment environment)
        {
            this.testArchive = testArchive;
            this.environment = environment;
            currentState = new CurrentStateModel(environment);
    }

        [HttpGet]
        public IActionResult Index()
        {
            return View("TestRunner", currentState);
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
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RunTests()
        {
            foreach (var assemblyPath in Directory.EnumerateFiles($"{environment.WebRootPath}/Temp"))
            {
                try
                {
                    var results = TestRunner.Test(assemblyPath);
                    var assemblyName = Path.GetFileName(assemblyPath);
                    var testedAssembly = testArchive.AssemblyModels.FirstOrDefault(a => a.Name == assemblyName);

                    if (testedAssembly == null)
                    {
                        testedAssembly = testArchive.AssemblyModels.Add(new AssemblyModel { Name = assemblyName }).Entity;
                        testArchive.SaveChanges();
                    }

                    foreach (var result in results)
                    {
                        var test = new TestModel
                        {
                            Name = result.Name,
                            ClassName = result.ClassName,
                            IsPassed = result.IsPassed,
                            IsIgnored = result.IsIgnored,
                            IgnoreReason = result.IgnoreReason,
                            RunTime = result.RunTime,
                            AssemblyModel = testedAssembly
                        };

                        currentState.Tests.Add(test);
                        testedAssembly.TestModels.Add(test);
                        testArchive.SaveChanges();
                    }
                }
                catch (TestRunnerException e)
                {
                    return View("TestRunnerError", e.Message);
                }
            }

            return View("TestRunner", currentState);
        }

        public IActionResult ClearCurrentAssemblies()
        {
            var tempDirectory = new DirectoryInfo($"{environment.WebRootPath}/Temp");

            foreach (var file in tempDirectory.GetFiles())
            {
                file.Delete();
            }

            return RedirectToAction("Index");
        }

        public IActionResult History()
        {
            return View(testArchive.AssemblyModels.Include("TestModels").ToList());
        }
    }
}