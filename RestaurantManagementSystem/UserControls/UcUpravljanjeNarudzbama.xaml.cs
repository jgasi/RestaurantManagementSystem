using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            // Dictionary za praćenje zauzetih stolova po datumu
            Dictionary<DateTime, HashSet<int>> zauzetiStoloviPoDatumu = new Dictionary<DateTime, HashSet<int>>();

            foreach (var narudzba in narudzbe)
            {
                // Pronađi sve rezervacije koje odgovaraju vremenu narudžbe
                var rezervacije = await rezervacijaServices.GetAllRezervacijeByDatum(narudzba.datum_vrijeme);

                // Uzimamo sve jedinstvene ID-ove stolova
                var stolovi = rezervacije
                    .Where(r => r.datum_vrijeme == narudzba.datum_vrijeme)
                    .Select(r => r.Stol_id_stol)
                    .Distinct() // Uzimamo samo jedinstvene stolove
                    .ToList();

                int? stolId = null;

                // Provjeravamo koji su stolovi zauzeti za trenutni datum
                if (!zauzetiStoloviPoDatumu.ContainsKey(narudzba.datum_vrijeme.Value.Date))
                {
                    zauzetiStoloviPoDatumu[narudzba.datum_vrijeme.Value.Date] = new HashSet<int>();
                }

                foreach (var id in stolovi)
                {
                    if (!zauzetiStoloviPoDatumu[narudzba.datum_vrijeme.Value.Date].Contains(id))
                    {
                        stolId = id; // Ako stol nije zauzet, uzmi taj ID
                        zauzetiStoloviPoDatumu[narudzba.datum_vrijeme.Value.Date].Add(stolId.Value); // Dodaj stol u zauzete
                        break; // Izađi iz petlje jer smo našli stol
                    }
                }

                // Ako stol ID nije pronađen, postavi na 0
                if (stolId == null)
                {
                    // Ako nema slobodnog stola, dodajte novu logiku za dodavanje ID-a
                    stolId = zauzetiStoloviPoDatumu[narudzba.datum_vrijeme.Value.Date].Count + 1;
                    zauzetiStoloviPoDatumu[narudzba.datum_vrijeme.Value.Date].Add(stolId.Value); // Dodaj novog stol u zauzete
                }

                // Pravimo string s prilagodbama za prikaz
                var prilagodba = narudzba.Stavka_narudzbe
                    .Select(s => s.prilagodbe)
                    .FirstOrDefault(p => !string.IsNullOrEmpty(p));

                var prilagodbeZaPrikaz = prilagodba != null ? SplitPrilagodbe(prilagodba, 35) : string.Empty;


                // Pravimo ViewModel za narudžbu
                narudzbaViewModels.Add(new NarudzbaViewModel
                {
                    narudzba_Id = narudzba.id_narudzba,
                    datum_vrijeme = narudzba.datum_vrijeme,
                    racun = narudzba.racun,
                    status = narudzba.status,
                    prilagodbe = prilagodbeZaPrikaz,
                    stol_id = stolId.Value // Dodajemo izračunati stol ID
                });
            }

            dgNarudzbe.ItemsSource = narudzbaViewModels;
        }

        public string SplitPrilagodbe(string prilagodba, int maxLineLength)
        {
            StringBuilder result = new StringBuilder();
            int start = 0;

            while (start < prilagodba.Length)
            {
                // Dohvati maksimalno 35 znakova ili ostatak stringa ako je kraći
                int length = Math.Min(maxLineLength, prilagodba.Length - start);
                result.AppendLine(prilagodba.Substring(start, length));
                start += length;
            }

            return result.ToString();
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
