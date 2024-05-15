using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.Autentikacija
{
    public static class CurrentUser
    {
        public static Korisnik LoggedInUser { get; set; }

    }
}
