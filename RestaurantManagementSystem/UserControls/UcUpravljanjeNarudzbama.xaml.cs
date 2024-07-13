using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using GalaSoft.MvvmLight.Helpers;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjeNarudzbama : UserControl
    {
        private NarudzbaServices narudzbaServices = new NarudzbaServices();
        private RezervacijaServices rezervacijaServices = new RezervacijaServices();
        private List<NarudzbaViewModel> narudzbaViewModels;



        public UcUpravljanjeNarudzbama()
        {
            InitializeComponent();
            LoadNarudzbe();
        }

        private async void LoadNarudzbe()
        {
            DateTime? now = DateTime.Now;
            var narudzbe = await narudzbaServices.GetNarudzbeByDateNowAsync(now);
            narudzbaViewModels = new List<NarudzbaViewModel>();

            foreach (var narudzba in narudzbe)
            {
                // Pronađi sve rezervacije za određenog korisnika i datum
                var rezervacije = await rezervacijaServices.GetAllRezervacijeByKorisnikIdAndDatum(narudzba.Korisnik_id_korisnik, narudzba.datum_vrijeme);

                // Inicijaliziramo listu stolova za ovu narudžbu
                var stolovi = new List<int>();

                // Prolazimo kroz sve rezervacije i dodajemo ID stola u listu
                foreach (var rezervacija in rezervacije)
                {
                    if (rezervacija.Stol_id_stol != 0 && !stolovi.Contains(rezervacija.Stol_id_stol))
                    {
                        stolovi.Add(rezervacija.Stol_id_stol);
                    }
                }

                // Pravimo string s prilagodbama za prikaz
                var prilagodbe = string.Join(", ", narudzba.Stavka_narudzbe.Select(s => s.prilagodbe));

                // Pravimo ViewModel za narudžbu, dodajući sve stolove koji su rezervirani za ovu narudžbu
                foreach (var stolId in stolovi)
                {
                    narudzbaViewModels.Add(new NarudzbaViewModel
                    {
                        narudzba_Id = narudzba.id_narudzba,
                        datum_vrijeme = narudzba.datum_vrijeme,
                        racun = narudzba.racun,
                        status = narudzba.status,
                        prilagodbe = prilagodbe,
                        stol_id = stolId
                    });
                }
            }

            dgNarudzbe.ItemsSource = narudzbaViewModels;
        }

        private async void RadioButton_Status_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                NarudzbaViewModel narudzbaViewModel = dgNarudzbe.SelectedItem as NarudzbaViewModel;
                if (narudzbaViewModel != null)
                {
                    narudzbaViewModel.status = radioButton.Content.ToString();

                    try
                    {
                        Narudzba narudzbaa = await narudzbaServices.GetNarudzbuByIdAsync(narudzbaViewModel.narudzba_Id);
                        narudzbaa.status = radioButton.Content.ToString();
                        narudzbaServices.UpdateNarudzbu(narudzbaa);

                        // Ovdje možete izravno ažurirati narudzbaViewModels ako je potrebno
                        narudzbaViewModels.FirstOrDefault(n => n.narudzba_Id == narudzbaa.id_narudzba).status = narudzbaa.status;

                        dgNarudzbe.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Greška pri ažuriranju narudžbe: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    public class NarudzbaViewModel
    {
        public int narudzba_Id { get; set; }
        public DateTime? datum_vrijeme { get; set; }
        public string racun { get; set; }
        public string status { get; set; }
        public string prilagodbe { get; set; }
        public int stol_id { get; set; }
    }
}
