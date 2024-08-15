using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcMojeRezervacije : UserControl
    {
        private Korisnik korisnik;
        private NarudzbaServices narudzbaServices = new NarudzbaServices();
        private RezervacijaServices rezervacijaServices = new RezervacijaServices();
        private Stavka_narudzbeServices stavka_NarudzbeServices = new Stavka_narudzbeServices();

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

        private async void OtkaziButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Jeste li sigurni da želite otkazati rezervaciju?", "Potvrda", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Button button = sender as Button;
                if (button != null)
                {
                    Narudzba narudzba = button.DataContext as Narudzba;
                    DateTime? vrijeme = narudzba.datum_vrijeme;
                    int idKorisnika = narudzba.Korisnik_id_korisnik;


                    if(narudzba != null)
                    {
                        await stavka_NarudzbeServices.RemoveStavkeNarudzbeByIdAsync(narudzba.id_narudzba);
                        await rezervacijaServices.RemoveRezervacijuPoDatumAndIdAsync(vrijeme, idKorisnika);
                        narudzbaServices.RemoveNarudzbu(narudzba);

                        LoadRezervacije();
                    }
                }
            }
        }
    }
}
