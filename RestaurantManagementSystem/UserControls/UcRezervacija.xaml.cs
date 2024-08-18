using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcRezervacija : UserControl
    {
        private JeloServices _jeloServices = new JeloServices();
        private PiceServices _piceServices = new PiceServices();
        private RezervacijaServices _rezervacijaServices = new RezervacijaServices();
        private StolServices _stolServices = new StolServices();
        private NarudzbaServices _narudzbaServices = new NarudzbaServices();
        private Stavka_narudzbeServices _stavka_narudzbeServices = new Stavka_narudzbeServices();
        private List<Jelo> _filteredFoodItems;
        private List<Pice> _filteredDrinkItems;
        private ObservableCollection<Jelo> _selectedFoodItems = new ObservableCollection<Jelo>();
        private ObservableCollection<Pice> _selectedDrinkItems = new ObservableCollection<Pice>();


        private Korisnik korisnik;

        private IMemoryCache _cache;

        private decimal _ukupnaCijena;


        public UcRezervacija(Korisnik trenutniKorisnik)
        {
            InitializeComponent();
            InitializeFilters();

            _cache = new MemoryCache(new MemoryCacheOptions());

            LoadJelaAndPicaIntoCache();
            korisnik = trenutniKorisnik;

            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            decimal total = 0;
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;

            foreach (var jelo in _selectedFoodItems)
            {
                if (decimal.TryParse(jelo.cijena, System.Globalization.NumberStyles.Any, cultureInfo, out decimal cijenaJelo))
                {
                    total += cijenaJelo;
                }
            }

            foreach (var pice in _selectedDrinkItems)
            {
                if (decimal.TryParse(pice.cijena, System.Globalization.NumberStyles.Any, cultureInfo, out decimal cijenaPice))
                {
                    total += cijenaPice;
                }
            }

            _ukupnaCijena = total;
            UpdateTotalPriceTextBlock();
        }



        private void UpdateTotalPriceTextBlock()
        {
            TotalPriceTextBlock.Text = _ukupnaCijena.ToString();
        }


        private async void LoadJelaAndPicaIntoCache()
        {
            // Check and retrieve food items from cache
            if (!_cache.TryGetValue("Jela", out List<Jelo> jela))
            {
                jela = await _jeloServices.GetAllJelaAsync();
                _cache.Set("Jela", jela);
            }

            // Check and retrieve drink items from cache
            if (!_cache.TryGetValue("Pica", out List<Pice> pica))
            {
                pica = await _piceServices.GetAllPicaAsync();
                _cache.Set("Pica", pica);
            }
        }

        private void InitializeFilters()
        {
            // Set up event handlers for filtering
            FoodFilterTextBox.TextChanged += FoodFilterTextBox_TextChanged;
            DrinkFilterTextBox.TextChanged += DrinkFilterTextBox_TextChanged;
        }

        private async void FoodFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = FoodFilterTextBox.Text.Trim();
            if (filter.Length >= 3)
            {
                // Attempt to retrieve from cache
                if (_cache.TryGetValue("FilteredFoodItems_" + filter, out List<Jelo> cachedFilteredFoodItems))
                {
                    _filteredFoodItems = cachedFilteredFoodItems;
                }
                else
                {
                    // If not in cache, fetch from database
                    _filteredFoodItems = await _jeloServices.GetJelaByNameAsync(filter);
                    _cache.Set("FilteredFoodItems_" + filter, _filteredFoodItems);
                }

                FoodListView.ItemsSource = _filteredFoodItems;
            }
            else
            {
                FoodListView.ItemsSource = null;
                _filteredFoodItems = null;
            }
        }

        private async void DrinkFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = DrinkFilterTextBox.Text.Trim();
            if (filter.Length >= 3)
            {
                // Attempt to retrieve from cache
                if (_cache.TryGetValue("FilteredDrinkItems_" + filter, out List<Pice> cachedFilteredDrinkItems))
                {
                    _filteredDrinkItems = cachedFilteredDrinkItems;
                }
                else
                {
                    // If not in cache, fetch from database
                    _filteredDrinkItems = await _piceServices.GetAllPicaByNameAsync(filter);
                    _cache.Set("FilteredDrinkItems_" + filter, _filteredDrinkItems);
                }

                DrinkListView.ItemsSource = _filteredDrinkItems;
            }
            else
            {
                DrinkListView.ItemsSource = null;
                _filteredDrinkItems = null;
            }
        }

        private void AddSelectedJelo(Jelo selectedJelo)
        {
            _selectedFoodItems.Add(selectedJelo);
            SelectedFoodItemsControl.ItemsSource = null; // Clear ItemsSource
            SelectedFoodItemsControl.ItemsSource = _selectedFoodItems;
            CalculateTotalPrice(); // Update total price

        }

        private void AddSelectedPice(Pice selectedPice)
        {
            _selectedDrinkItems.Add(selectedPice);
            SelectedDrinkItemsControl.ItemsSource = null; // Clear ItemsSource
            SelectedDrinkItemsControl.ItemsSource = _selectedDrinkItems;
            CalculateTotalPrice(); // Update total price

        }

        private void FoodListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FoodListView.SelectedItem != null)
            {
                Jelo selectedJelo = FoodListView.SelectedItem as Jelo;
                AddSelectedJelo(selectedJelo);
                FoodListView.SelectedItem = null; // Clear selection
            }
        }

        private void DrinkListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrinkListView.SelectedItem != null)
            {
                Pice selectedPice = DrinkListView.SelectedItem as Pice;
                AddSelectedPice(selectedPice);
                DrinkListView.SelectedItem = null; // Clear selection
            }
        }

        private async void Rezerviraj_Click(object sender, RoutedEventArgs e)
        {
            // Inicijaliziraj listu za poruke o greškama
            List<string> errorMessages = new List<string>();

            // Provjeri jesu li jela ili pića odabrani
            if (_selectedFoodItems.Count == 0 && _selectedDrinkItems.Count == 0)
            {
                errorMessages.Add("Odaberi barem jedno jelo ili piće!");
            }

            // Provjeri je li datum i vrijeme rezervacije odabrano i jel u budućnosti
            DateTime? selectedDateTime = ReservationDateTimePicker.Value;
            if (selectedDateTime == null)
            {
                errorMessages.Add("Odaberi datum i vrijeme rezervacije!");
            }
            else if (selectedDateTime <= DateTime.Now)
            {
                errorMessages.Add("Datum i vrijeme rezervacije moraju biti u budućnosti!");
            }

            // Ako postoje poruke o greškama, prikaži ih
            if (errorMessages.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Join("\n", errorMessages));
                return;
            }

            // Calculate end time as 2 hours after selectedDateTime
            DateTime endTime = selectedDateTime.Value.AddHours(2);

            // Retrieve all reservations within the selected time slot
            var sveRezervacije = await _rezervacijaServices.GetAllRezervacijeAsync();
            var overlappingReservations = sveRezervacije
                .Where(r => r.datum_vrijeme < endTime && r.datum_vrijeme.Value.AddHours(2) > selectedDateTime)
                .ToList();


            // Get available tables (stolovi)
            var stolovi = await _stolServices.GetSlobodneStolove();

            // Find the first available table that is not reserved in the overlapping time slot
            int? tableId = null;
            foreach (var stol in stolovi)
            {
                bool isTableAvailable = !overlappingReservations.Any(r => r.Stol_id_stol == stol.id_stol);
                if (isTableAvailable)
                {
                    tableId = stol.id_stol;
                    break;
                }
            }

            if (tableId == null)
            {
                // No available table found for the selected time slot
                System.Windows.Forms.MessageBox.Show("Nema dostupnog stola za odabrani termin.");
                return;
            }

            Rezervacija novaRezervacija = new Rezervacija
            {
                datum_vrijeme = selectedDateTime.Value,
                Korisnik_id_korisnik = korisnik.id_korisnik,
                Stol_id_stol = tableId.Value
            };

            _rezervacijaServices.AddRezervaciju(novaRezervacija);

            Narudzba narudzba = new Narudzba
            {
                datum_vrijeme = selectedDateTime.Value,
                racun = GenerateRacun(),
                status = "Zaprimljeno",
                Korisnik_id_korisnik = korisnik.id_korisnik
            };

            Narudzba narudzbaNova = await _narudzbaServices.AddNarudzbuAsync(narudzba);


            foreach (var jelo in _selectedFoodItems)
            {
                Stavka_narudzbe stavka_Narudzbe = new Stavka_narudzbe
                {
                    kolicina = "1",
                    prilagodbe = txtPrilagodbe.Text,
                    Narudzba_id_narudzba = narudzbaNova.id_narudzba,
                    Jelo_id_jelo = jelo.id_jelo,
                    Pice_id_pice = null
                };
                _stavka_narudzbeServices.AddStavkeNarudzbe(stavka_Narudzbe);
            }

            foreach (var pice in _selectedDrinkItems)
            {
                Stavka_narudzbe stavka_Narudzbe = new Stavka_narudzbe
                {
                    kolicina = "1",
                    prilagodbe = txtPrilagodbe.Text,
                    Narudzba_id_narudzba = narudzbaNova.id_narudzba,
                    Jelo_id_jelo = null,
                    Pice_id_pice = pice.id_pice
                };
                _stavka_narudzbeServices.AddStavkeNarudzbe(stavka_Narudzbe);
            }

            System.Windows.Forms.MessageBox.Show("Rezervacija je kreirana. Vidimo se!");

            _ukupnaCijena = 0;
            TotalPriceTextBlock.Text = _ukupnaCijena.ToString();

            _selectedFoodItems.Clear();
            _selectedDrinkItems.Clear();
            SelectedFoodItemsControl.ItemsSource = null;
            SelectedDrinkItemsControl.ItemsSource = null;
            txtPrilagodbe.Text = "";
        }


        private string GenerateRacun()
        {
            var racunDetails = new System.Text.StringBuilder();
            racunDetails.AppendLine("Narudžba:");
            foreach (var jelo in _selectedFoodItems)
            {
                racunDetails.AppendLine($"{jelo.naziv} - {jelo.cijena} EUR");
            }
            foreach (var pice in _selectedDrinkItems)
            {
                racunDetails.AppendLine($"{pice.naziv} - {pice.cijena} EUR");
            }
            racunDetails.AppendLine($"Ukupna cijena: {_ukupnaCijena} EUR");
            return racunDetails.ToString();
        }

        private void RemoveJeloButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Jelo selectedJelo)
            {
                _selectedFoodItems.Remove(selectedJelo);
                SelectedFoodItemsControl.ItemsSource = null;
                SelectedFoodItemsControl.ItemsSource = _selectedFoodItems;
                CalculateTotalPrice(); // Update total price
            }
        }

        private void RemovePiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Pice selectedPice)
            {
                _selectedDrinkItems.Remove(selectedPice);
                SelectedDrinkItemsControl.ItemsSource = null;
                SelectedDrinkItemsControl.ItemsSource = _selectedDrinkItems;
                CalculateTotalPrice();
            }
        }

    }
}
