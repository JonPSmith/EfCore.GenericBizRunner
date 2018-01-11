using System.Globalization;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DatabaseServices.Concrete;

namespace EfCoreInAction.Controllers
{
    public class AdminController : BaseTraceController
    {
        private readonly EfCoreContext _context;
        private readonly IHostingEnvironment _env;

        public AdminController(EfCoreContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //------------------------------------------------
        //Amdin commands that are called from the top menu

        public IActionResult ResetDatabase()
        {
            Request.ThrowErrorIfNotLocal();

            _context.DevelopmentEnsureDeleted();
            _context.DevelopmentEnsureCreated();
            var numBooks = _context.SeedDatabase(_env.WebRootPath);
            SetupTraceInfo();
            return View("BookUpdated", $"Successfully reset the database and added {numBooks} books.");
        }

        public IActionResult ResetOrders()
        {
            Request.ThrowErrorIfNotLocal();

            _context.ResetOrders();
            SetupTraceInfo();
            return View("BookUpdated", $"Successfully reset the customer orders.");
        }

    }
}
