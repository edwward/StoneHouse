using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]       //tento controller muze pouzivat pouze superadmin
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public ProductTypesController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View(this.db.ProductTypes.ToList());     //get all product types from Db
        }

        //GET Create Action Method
        public IActionResult Create()
        {
            return View();
        }

        //POST Create action Method, product type will be posted back as parameter
        [HttpPost]
        [ValidateAntiForgeryToken] //security measure - token je testovan zda je validni
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)     //testuje, zda jsou data anotace (jako required treba) v modelu platne
            {
                this.db.Add(productTypes);
                await this.db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //GET Edit Action Method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await this.db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
            
           
        }

        //POST Edit action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypes productTypes)
        {
            if (id != productTypes.Id)      //both ids should be the same
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                this.db.Update(productTypes);  //zde updatujeme pouze jednu vlastnost (name), pokud jich je vic (coz je vetsinou), je to slozitejsi
                await this.db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
            
        }

        //GET Details Action Method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await this.db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        //GET Delete Action Method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await this.db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        //POST Delete action Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTypes = await this.db.ProductTypes.FindAsync(id);
            this.db.ProductTypes.Remove(productTypes);
            await this.db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
