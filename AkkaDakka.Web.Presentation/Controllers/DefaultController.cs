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
        public IActionResult Index(int slide = 0)
        {
            if (slide < 0 || slide > Slides.Get().Length - 1) throw new Exception("Get lost!");

            return View(Slides.Get(slide).View, new _LayoutModel(Slides.Get(), slide));
        }
    }
}