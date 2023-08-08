using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora_App.Controllers
{
    public class AgoraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
