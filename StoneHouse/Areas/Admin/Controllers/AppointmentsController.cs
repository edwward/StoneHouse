using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using StoneHouse.Models;
using StoneHouse.Models.ViewModel;
using StoneHouse.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StoneHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]  //controller je zpristupneny adminem i superadminem
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private int PageSize = 3;

        //public object ApplicationUser { get; private set; }

        public AppointmentsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Index(int productPage = 1, string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)  //parametry mohou nebo nemusi byt zadany pri volani metody
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);  //get user identity id

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Models.Appointments>()
            };

            StringBuilder param = new StringBuilder();      //build string for pagination taghelper url

            param.Append("/Admin/Appointments?productPage=:"); //default url
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }

            appointmentVM.Appointments = this.db.Appointments.Include(a => a.SalesPerson).ToList();

            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();        //if admin is salesperson, they can only see appointments assigned to them
            }

            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }

            }

            var count = appointmentVM.Appointments.Count;  //how many appointments together after search criteria

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDate)  //order by date
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            return View(appointmentVM);
        }

        //GET Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in this.db.Products
                                                      join a in this.db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = this.db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = this.db.ApplicationUser.ToList(),
                Products = productList.ToList()
            };
            return View(objAppointmentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {

            objAppointmentVM.Appointment.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate
                        .AddHours(objAppointmentVM.Appointment.AppointmentTime.Hour)
                        .AddMinutes(objAppointmentVM.Appointment.AppointmentTime.Minute);

            var appointmentFromDb = this.db.Appointments.Where(a => a.Id == objAppointmentVM.Appointment.Id).FirstOrDefault();

            appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
            appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
            appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointment.CustomerPhoneNumber;
            appointmentFromDb.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate;
            appointmentFromDb.isConfirmed = objAppointmentVM.Appointment.isConfirmed;
            if (User.IsInRole(SD.SuperAdminEndUser))
            {
                appointmentFromDb.SalesPersonId = objAppointmentVM.Appointment.SalesPersonId;
            }
            this.db.SaveChanges();

            return RedirectToAction(nameof(Index));

            //return View(objAppointmentVM);
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in this.db.Products
                                                      join a in this.db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = this.db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = this.db.ApplicationUser.ToList(),
                Products = productList.ToList()
            };
            return View(objAppointmentVM);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productList = (IEnumerable<Products>)(from p in this.db.Products
                                                      join a in this.db.ProductsSelectedForAppointment
                                                      on p.Id equals a.ProductId
                                                      where a.AppointmentId == id
                                                      select p).Include("ProductTypes");

            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {
                Appointment = this.db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = this.db.ApplicationUser.ToList(),
                Products = productList.ToList()
            };
            return View(objAppointmentVM);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await this.db.Appointments.FindAsync(id);
            this.db.Appointments.Remove(appointment);
            await this.db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
