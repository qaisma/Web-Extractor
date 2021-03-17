using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_Extractor.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View("Index");
        }

        public ViewResult Task1()
        {
            return View("Task1");
        }

        public ViewResult Task2()
        {
            return View("Task2");
        }
    }
}