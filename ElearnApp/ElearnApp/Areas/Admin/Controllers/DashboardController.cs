using ElearnApp.Data;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
       
        public async Task<ActionResult> Index()
        {

            return View();
        }
    }
}
