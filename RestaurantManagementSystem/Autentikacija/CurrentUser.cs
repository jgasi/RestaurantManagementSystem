using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.Autentikacija
{
    public static class CurrentUser
    {
        public static Korisnik LoggedInUser { get; set; }

        public static void Logout()
        {
            LoggedInUser = null;
        }
    }
}
