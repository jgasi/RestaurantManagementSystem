﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private List<Jelo> _filteredFoodItems;
        private List<Pice> _filteredDrinkItems;
        private List<Jelo> _selectedFoodItems = new List<Jelo>();
        private List<Pice> _selectedDrinkItems = new List<Pice>();

        private Korisnik korisnik;

        private IMemoryCache _cache;

        private decimal _ukupnaCijena;


        public UcRezervacija(Korisnik trenutniKorisnik)
        {
            InitializeComponent();
            InitializeFilters();

            // Initialize MemoryCache
            _cache = new MemoryCache(new MemoryCacheOptions());

            // Asynchronously load food and drink items into cache if not already loaded
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
            // Check if at least one food item or drink is selected
            if (_selectedFoodItems.Count == 0 && _selectedDrinkItems.Count == 0)
            {
                return;
            }

            // Get the selected reservation date and time
            DateTime? selectedDateTime = ReservationDateTimePicker.Value;

            if (selectedDateTime == null)
            {
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
                return;
            }

            // Save the reservation
            Rezervacija novaRezervacija = new Rezervacija
            {
                datum_vrijeme = selectedDateTime.Value,
                Korisnik_id_korisnik = korisnik.id_korisnik,
                Stol_id_stol = tableId.Value
            };

            _rezervacijaServices.AddRezervaciju(novaRezervacija);

            System.Windows.Forms.MessageBox.Show("Rezervacija je kreirana. Vidimo se!");

            _ukupnaCijena = 0;
            TotalPriceTextBlock.Text = _ukupnaCijena.ToString();

            // Clear selected items after successful reservation
            _selectedFoodItems.Clear();
            _selectedDrinkItems.Clear();
            SelectedFoodItemsControl.ItemsSource = null; // Clear ItemsSource
            SelectedDrinkItemsControl.ItemsSource = null; // Clear ItemsSource

            // Optionally, you can perform any post-reservation logic here
        }




        // Additional functionality added
        private void AddJeloButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Jelo selectedJelo)
            {
                _selectedFoodItems.Add(selectedJelo);
                SelectedFoodItemsControl.ItemsSource = null;
                SelectedFoodItemsControl.ItemsSource = _selectedFoodItems;
            }
        }

        private void AddPiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Pice selectedPice)
            {
                _selectedDrinkItems.Add(selectedPice);
                SelectedDrinkItemsControl.ItemsSource = null;
                SelectedDrinkItemsControl.ItemsSource = _selectedDrinkItems;
            }
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
