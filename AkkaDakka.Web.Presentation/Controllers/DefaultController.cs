using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaBank.Web.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace AkkaBank.Web.Presentation.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Pages/index.cshtml", new _LayoutModel(Steps.Get(), 0));
        }
    }
}