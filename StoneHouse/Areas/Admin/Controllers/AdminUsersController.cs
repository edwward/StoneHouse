using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {

        private readonly ApplicationDbContext db;

        public AdminUsersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View(this.db.ApplicationUser.ToList());
        }
    }
}

