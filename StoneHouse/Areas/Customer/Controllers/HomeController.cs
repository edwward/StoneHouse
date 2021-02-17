using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoneHouse.Data;
using StoneHouse.Extensions;
using StoneHouse.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();  //get all products from Db
            return View(productList);  //return list of products
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == id).FirstOrDefaultAsync();  
            return View(product);  
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart"); //is anything store in session? ssShoppingCart = nazev session
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>(); //vytvor novy list
            }
            lstShoppingCart.Add(id);    //pridej productid to listu
            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart); //zacni session

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        //Remove action method
        public IActionResult Remove(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart.Count > 0)
            {
                if (lstShoppingCart.Contains(id))
                {
                    lstShoppingCart.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);
            return RedirectToAction(nameof(Index));
        }


        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
