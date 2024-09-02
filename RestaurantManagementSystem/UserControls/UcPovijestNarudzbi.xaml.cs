using System.Collections.Generic;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcPovijestNarudzbi : UserControl
    {
        private Korisnik korisnik;
        private NarudzbaServices narudzbaServices = new NarudzbaServices();

        public UcPovijestNarudzbi(Korisnik trenutniKorisnik)
        {
            InitializeComponent();
            korisnik = trenutniKorisnik;
            LoadNarudzbe();
        }

        private async void LoadNarudzbe()
        {
            List<Narudzba> narudzbe = await narudzbaServices.GetAllNarudzbeByKorisnikAsync(korisnik.id_korisnik);
            dgNarudzbe.ItemsSource = narudzbe;
        }
    }
}
