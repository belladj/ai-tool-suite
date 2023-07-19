using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ai_tool_suite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult ChatWithPDF()
        {
            return View();
        }

        public ActionResult SQLTable()
        {
            return RedirectToAction("Index", "Users");
        }

        public ActionResult PromptTester()
        {
            return RedirectToAction("Index", "Prompts");
        }
    }
}