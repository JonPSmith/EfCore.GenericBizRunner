using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.Logger;

namespace EfCoreInAction.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)   
        {                                              
            _context = context;                        
        }                                              

        public IActionResult Index                     
            (SortFilterPageOptions options)            
        {
            var listService =                          
                new ListBooksService(_context);        

            var bookList = listService                 
                .SortFilterPage(options)               
                .ToList();                             

            SetupTraceInfo();           //Thsi makes the logging display work

            return View(new BookListCombinedDto         
                (options, bookList));                   
        }


        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent(SortFilterPageOptions options)         
        {
            var service = new BookFilterDropdownService(_context);

            var traceIdent = HttpContext.TraceIdentifier; //This makes the logging display work

            return Json(                            
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent,
                service.GetFilterDropDownValues(    
                    options.FilterBy)));            
        }


        public IActionResult About()
        {
            var isLocal = Request.IsLocal();
            return View(isLocal);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
