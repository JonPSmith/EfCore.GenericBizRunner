using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using EfCoreInAction.Helpers;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.Logger;
using ExampleWebApp.Helpers;

namespace EfCoreInAction.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public HomeController(EfCoreContext context)   
        {                                              
            _context = context;                        
        }                                              

        public IActionResult Index(SortFilterPageOptions options)            
        {
            var listService =                          
                new ListBooksService(_context);        

            var bookList = listService                 
                .SortFilterPage(options)               
                .ToList();                             

            SetupTraceInfo();           //Thsi makes the logging display work

            return View(new BookListCombinedDto(options, bookList));                   
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

        //----------------------------------------------
        //Now the code to alter the book

        public IActionResult ChangePubDate(int id, [FromServices]IChangePubDateService service) 
        {
            var dto = service.GetOriginal(id); 
            SetupTraceInfo();
            return View(dto); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePubDate(ChangePubDateDto dto, [FromServices]IChangePubDateService service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            service.UpdateBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully changed publication date");
        }

        public IActionResult AddPromotion(int id, [FromServices]IAddRemovePromotionService service)
        {
            var dto = service.GetOriginal(id);
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPromotion(AddRemovePromotionDto dto, [FromServices]IAddRemovePromotionService service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            service.AddPromotion(dto);
            if (service.Status.HasErrors)
            {
                service.Status.CopyErrorsToModelState(ModelState, dto);
                return View(dto);
            }
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added/changed a promotion");
        }


        public IActionResult RemovePromotion(int id, [FromServices]IAddRemovePromotionService service)
        {
            var dto = service.GetOriginal(id);
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePromotion(AddRemovePromotionDto dto, [FromServices]IAddRemovePromotionService service)
        {
            service.RemovePromotion(dto.BookId);
            if (service.Status.HasErrors)
            SetupTraceInfo();
            return View("BookUpdated", "Successfully removed a promotion");
        }


        public IActionResult AddReview(int id, [FromServices]IAddReviewService service)
        {
            var dto = service.GetOriginal(id);
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(AddReviewDto dto, [FromServices]IAddReviewService service)
        {


            var book = service.AddReviewToBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added a review");
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
