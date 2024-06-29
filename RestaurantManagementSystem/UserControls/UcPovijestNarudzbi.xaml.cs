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
    /// <summary>
    /// Interaction logic for UcPovijestNarudzbi.xaml
    /// </summary>
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
