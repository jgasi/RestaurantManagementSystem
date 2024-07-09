using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcPregledJelovnika : UserControl
    {
        private static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private JeloServices jeloServices = new JeloServices();
        public Jelo SelectedJelo { get; set; }
        private int currentPage = 0;
        private int itemsPerPage = 3;
        private bool isLoading = false;
        private Korisnik trenutniKorisnik;

        public ObservableCollection<Jelo> CurrentPageJela { get; set; }

        public UcPregledJelovnika(Korisnik korisnik)
        {
            InitializeComponent();
            CurrentPageJela = new ObservableCollection<Jelo>();
            DataContext = this;
            PrikaziStranicuFirstThree();
            UcitajSvaJelaAsync();
            trenutniKorisnik = korisnik;
        }

        private async void PrikaziStranicuFirstThree()
        {
            var allItems = new List<Jelo>();

            var first = await jeloServices.GetFirstThreeJelaAsync(2);
            allItems.AddRange(first);

            var second = await jeloServices.GetFirstThreeJelaAsync(3);
            allItems.AddRange(second);

            var third = await jeloServices.GetFirstThreeJelaAsync(4);
            allItems.AddRange(third);

            foreach (var item in allItems)
            {
                if (!IsJeloCached(item.id_jelo))
                {
                    CurrentPageJela.Add(item);
                    AddJeloToCache(item);
                }
            }
            loadingText.Visibility = Visibility.Collapsed;
        }

        private async void UcitajSvaJelaAsync()
        {
            isLoading = true;

            try
            {
                if (!cache.TryGetValue("SvaJelaCache", out List<Jelo> SvaJelaCache))
                {
                    SvaJelaCache = await jeloServices.GetAllJelaAsync();
                    cache.Set("SvaJelaCache", SvaJelaCache, TimeSpan.FromMinutes(30));
                }
            }
            finally
            {
                isLoading = false;
                PrikaziStranicu();
            }
        }

        private bool IsJeloCached(int jeloId)
        {
            return cache.TryGetValue("SvaJelaCache", out List<Jelo> SvaJelaCache) && SvaJelaCache.Any(j => j.id_jelo == jeloId);
        }

        private void AddJeloToCache(Jelo jelo)
        {
            if (cache.TryGetValue("SvaJelaCache", out List<Jelo> SvaJelaCache))
            {
                SvaJelaCache.Add(jelo);
                cache.Set("SvaJelaCache", SvaJelaCache, TimeSpan.FromMinutes(30));
            }
        }

        private void PrikaziStranicu()
        {
            if (isLoading)
            {
                loadingText.Visibility = Visibility.Visible;
                return;
            }

            CurrentPageJela.Clear();

            if (cache.TryGetValue("SvaJelaCache", out List<Jelo> SvaJelaCache))
            {
                var items = SvaJelaCache.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();

                foreach (var item in items)
                {
                    CurrentPageJela.Add(item);
                }

                loadingText.Visibility = Visibility.Collapsed;
            }
            else
            {
                loadingText.Visibility = Visibility.Visible;
                CurrentPageJela.Clear();
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = (currentPage + 1) * itemsPerPage < (cache.Get<List<Jelo>>("SvaJelaCache")?.Count ?? 0);
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                PrikaziStranicu();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
            {
                currentPage++;
                PrikaziStranicu();
            }
            else
            {
                CurrentPageJela.Clear();
                loadingText.Visibility = Visibility.Visible;
                currentPage++;
            }
        }

        private void Jelo_Click(object sender, RoutedEventArgs e)
        {
            SelectedJelo = (sender as FrameworkElement)?.DataContext as Jelo;

            if (SelectedJelo != null)
            {
                UcDetaljiJela detaljiJela = new UcDetaljiJela(SelectedJelo, trenutniKorisnik);
                detaljiJela.DataContext = SelectedJelo;
                glavniGrid.Children.Clear();
                glavniGrid.Children.Add(detaljiJela);
            }
        }
    }
}
