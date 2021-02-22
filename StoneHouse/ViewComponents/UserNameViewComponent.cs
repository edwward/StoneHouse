using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoneHouse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StoneHouse.ViewComponents
{
    //view component je neco jako partialview ale novejsi, zde bude pouzit pro zobrazeni uziv. jmena misto emailu pri prihlaseni na navbaru
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            this.db = db;
        }

        //get user id from claims.Value
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userFromDb = await this.db.ApplicationUser.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();
            return View(userFromDb);
        }
    }
}
