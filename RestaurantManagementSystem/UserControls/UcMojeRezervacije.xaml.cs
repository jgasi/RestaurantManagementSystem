using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcMojeRezervacije : UserControl
    {
        private Korisnik korisnik;
        private NarudzbaServices narudzbaServices = new NarudzbaServices();

        public UcMojeRezervacije(Korisnik trenutniKorisnik)
        {
            InitializeComponent();
            korisnik = trenutniKorisnik;
            LoadRezervacije();
        }

        private async void LoadRezervacije()
        {
            List<Narudzba> narudzbe = await narudzbaServices.GetAllNarudzbeByKorisnikAsync(korisnik.id_korisnik);
            DateTime currentTime = DateTime.Now;
            var upcomingNarudzbe = narudzbe.Where(n => n.datum_vrijeme > currentTime).ToList();
            dgRezervacije.ItemsSource = upcomingNarudzbe;
        }
    }
}
