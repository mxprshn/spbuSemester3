using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Controllers
{
    /// <summary>
    /// Main app controller.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly TestArchive testArchive;
        private readonly IWebHostEnvironment environment;
        private CurrentStateModel currentState;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="testArchive">Test history database access object.</param>
        /// <param name="environment">Current environment.</param>
        public HomeController(TestArchive testArchive, IWebHostEnvironment environment)
        {
            this.testArchive = testArchive;
            this.environment = environment;
            currentState = new CurrentStateModel(environment);
        }

        /// <summary>
        /// Loads start page for test running.
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View("TestRunner", currentState);
        }

        /// <summary>
        /// Adds assembly file to Temp folder on server.
        /// </summary>
        /// <param name="file">File loading from form.</param>
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

        /// <summary>
        /// Runs tests in all assemblies in Temp folder and loads page with results. 
        /// </summary>
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
                catch (AggregateException e) when (e.InnerException.GetType() == typeof(TestRunnerException))
                {
                    return View("TestRunnerError", e.InnerException.Message);
                }
            }

            return View("TestRunner", currentState);
        }

        /// <summary>
        /// Deletes all assemblies in Temp folder.
        /// </summary>
        public IActionResult ClearCurrentAssemblies()
        {
            var tempDirectory = new DirectoryInfo($"{environment.WebRootPath}/Temp");

            foreach (var file in tempDirectory.GetFiles())
            {
                file.Delete();
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Loads page with test run history.
        /// </summary>
        public IActionResult History()
        {
            return View(testArchive.AssemblyModels.Include("TestModels").ToList());
        }
    }
}