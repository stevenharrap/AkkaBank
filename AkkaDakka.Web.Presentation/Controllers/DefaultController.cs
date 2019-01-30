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
        public IActionResult Index(int step = 0)
        {
            if (step < 0 || step > Steps.Get().Length - 1) throw new Exception("Get lost!");

            return View(Steps.Get(step).View, new _LayoutModel(Steps.Get(), step));
        }
    }
}