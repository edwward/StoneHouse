using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoneHouse.Models
{
    //diky dedeni z identityuser bude kazda property v applicationuser class pridana do tabulky aspnetusers
    //EF nevytvori pri pridani migration novou tabulku, ale pouze prida columns k tabulce aspnetusers
    //user bude bud admin nebo superadmin, ktery je jeste nad normalnim adminem
    public class ApplicationUser : IdentityUser 
    {
        [Display(Name = "Sales Person")]
        public string Name { get; set; }

        [NotMapped]  //nebude v Db
        public bool IsSuperAdmin { get; set; }
    }
}
