using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Extensions;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            this.db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        //Get Index Shopping Cart
        public async Task<IActionResult> Index()
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");  //retrieve all items from shoppingcart session
            if (lstShoppingCart == null)
            {
                return View(ShoppingCartVM);
            }
            if (lstShoppingCart.Count > 0)
            {
                foreach (int cartItem in lstShoppingCart)
                {
                    Products prod = this.db.Products.Include(p => p.SpecialTags).Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()  //ShoppingCartVM is already binded, no need for parameters
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");      //get the session to list of int

            //DATEPICKER JQUERY NEVRACI SPRAVNE DATUM
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate       //merge app.time and app.date to appointment itself (AppointmentDate)
                                                            .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                            .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);


            Appointments appointments = ShoppingCartVM.Appointments;  //save to appointments object
            this.db.Appointments.Add(appointments);
            this.db.SaveChanges();

            int appointmentId = appointments.Id;        //id was created when save to Db

            foreach (int productId in lstCartItems)  //for every itemid in shoppingcart
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,      //merge appointmentId with AppointmentId from ProductsSelectedForAppointment class
                    ProductId = productId               //viz vyse
                };
                this.db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);  //pridej do Db

            }
            this.db.SaveChanges();
            lstCartItems = new List<int>();  //empty listcart items
            HttpContext.Session.Set("ssShoppingCart", lstCartItems);  //begin new session for another shoppingcart

            //po vytvoreni appointment redirect to AppointmentConfirmation method in ShoppingCartcontroller and use Id you retrieve
            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });

        }

        //Remove action method
        public IActionResult Remove(int id)
        {
            List<int> lstCartItems = HttpContext.Session.Get<List<int>>("ssShoppingCart");

            if (lstCartItems.Count > 0)
            {
                if (lstCartItems.Contains(id))
                {
                    lstCartItems.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", lstCartItems);

            return RedirectToAction(nameof(Index));
        }

        //Get appointment method
        public IActionResult AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointments = this.db.Appointments.Where(a => a.Id == id).FirstOrDefault();  //fill shoppingcartVM based on appointmentId
            List<ProductsSelectedForAppointment> objProdList = this.db.ProductsSelectedForAppointment.Where(p => p.AppointmentId == id).ToList();       //retrieve all products in appointment and save to list

            foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
            {
                //add products to ShoppingCartVM - we have appointment and all products for appointment
                ShoppingCartVM.Products.Add(this.db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }

            return View(ShoppingCartVM);
        }

    }
}
