using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;
using StoneHouse.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]       //tento controller muze pouzivat pouze superadmin
    //ProductsController bude uzivat ProductsViewModel
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hostingEnvironment;

        [BindProperty]      //automaticky priradi ProductVM do action metod tam kde je treba, nemusim ho metode predavat v parametru
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;

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


        //Get : Products Create
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        //Post : Products Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()  //diky bindproperty nemusim predavat VM v parametru metody
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }

            this.db.Products.Add(ProductsVM.Products);
            await this.db.SaveChangesAsync();

            //Image being saved

            string webRootPath = this.hostingEnvironment.WebRootPath; //rootpath of application
            var files = HttpContext.Request.Form.Files;  //get all files that were uploaded from view

            var productsFromDb = this.db.Products.Find(ProductsVM.Products.Id);  //retrieve products from Db

            if (files.Count != 0)  //jsou tam nejake images?
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);  //najdi cestu k images
                var extension = Path.GetExtension(files[0].FileName);  //nnjdi extensions k images

                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))  //copy image to server, rename images according to productid
                {
                    files[0].CopyTo(filestream);  //move image to server and rename it
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension;  //update product from DB with the new path
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage); //add default image
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg");  //use default image
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg";  //update image on server with new path
            }
            await this.db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET : Edit product
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //get products from Db, include specialtabs and producttype, select single item based on id from this method parameter
            ProductsVM.Products = await this.db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        //Post : Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = this.hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = this.db.Products.Where(m => m.Id == ProductsVM.Products.Id).FirstOrDefault(); //find name of existing image

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image, we want to replace the old image with new one
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);  //get the folder of the image
                    var extensionNew = Path.GetExtension(files[0].FileName); //get the new image extension
                    var extensionOld = Path.GetExtension(productFromDb.Image);   //get the old image extension

                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extensionOld)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extensionOld));  //delete old file
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extensionNew), FileMode.Create)) //append new file to server
                    {
                        files[0].CopyTo(filestream);  //copy to server
                    }
                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extensionNew;
                }

                if (ProductsVM.Products.Image != null)
                {
                    productFromDb.Image = ProductsVM.Products.Image;
                }

                //update all other properties
                productFromDb.Name = ProductsVM.Products.Name;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.SpecialTagsID = ProductsVM.Products.SpecialTagsID;
                productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;
                await this.db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else 
            {
                return View(ProductsVM);
            }
            
        }


        //GET : Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await this.db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        //GET : Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await this.db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);
        }

        //POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = this.hostingEnvironment.WebRootPath;
            Products products = await this.db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }
                this.db.Products.Remove(products);  //remove entry from Db
                await this.db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }

}
