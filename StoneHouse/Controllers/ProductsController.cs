using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Controllers
{
    //ProductsController bude uzivat ProductsViewModel

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]      //automaticky priradi ProductVM do action metod tam kde je treba, nemusim ho metode predavat v parametru
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db)
        {
            this.db = db;
            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = this.db.ProductTypes.ToList(),
                SpecialTags = this.db.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = this.db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);   //pridej oba modely do modelu Products
            return View(await products.ToListAsync()); //zobraz vse v products
        }
    }

}
