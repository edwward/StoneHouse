using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    //model pro planovani schuzek ohledne zbozi
    public class Appointments
    {
        public int Id { get; set; }

        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; }

        [ForeignKey("SalesPersonId")]       //SalespersonID je foreign key pro tabulku ApplicationUser, sloupec SalesPerson
        public virtual ApplicationUser SalesPerson { get; set; }

        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        public DateTime AppointmentTime { get; set; } //nebude v Db

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Phone Number")]
        public string CustomerPhoneNumber { get; set; }

        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Is Confirmed")]
        public bool isConfirmed { get; set; }

    }
}
