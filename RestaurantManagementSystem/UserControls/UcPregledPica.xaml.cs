using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcPregledPica : UserControl
    {
        private static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        private PiceServices piceServices = new PiceServices();
        public Pice SelectedPice { get; set; }
        private int currentPage = 0;
        private int itemsPerPage = 3;
        private bool isLoading = false;
        private Korisnik trenutniKorisnik;

        public ObservableCollection<Pice> CurrentPagePica { get; set; }

        public UcPregledPica(Korisnik korisnik)
        {
            InitializeComponent();
            CurrentPagePica = new ObservableCollection<Pice>();
            DataContext = this;
            PrikaziStranicuFirstThree();
            UcitajSvaPicaAsync();
            trenutniKorisnik = korisnik;
        }

        private async void PrikaziStranicuFirstThree()
        {
            var allItems = new List<Pice>();

            var first = await piceServices.GetFirstThreePicaAsync();
            allItems.AddRange(first);

            foreach (var item in allItems)
            {
                if (!IsPiceCached(item.id_pice))
                {
                    CurrentPagePica.Add(item);
                    AddPiceToCache(item);
                }
            }
            loadingText.Visibility = Visibility.Collapsed;
        }

        private async void UcitajSvaPicaAsync()
        {
            isLoading = true;

            try
            {
                if (!cache.TryGetValue("SvaPicaCache", out List<Pice> SvaPicaCache))
                {
                    SvaPicaCache = await piceServices.GetAllPicaAsync();
                    cache.Set("SvaPicaCache", SvaPicaCache, TimeSpan.FromMinutes(30));
                }
            }
            finally
            {
                isLoading = false;
                PrikaziStranicu();
            }
        }

        private bool IsPiceCached(int piceId)
        {
            return cache.TryGetValue("SvaPicaCache", out List<Pice> SvaPicaCache) && SvaPicaCache.Any(p => p.id_pice == piceId);
        }

        private void AddPiceToCache(Pice pice)
        {
            if (cache.TryGetValue("SvaPicaCache", out List<Pice> SvaPicaCache))
            {
                SvaPicaCache.Add(pice);
                cache.Set("SvaPicaCache", SvaPicaCache, TimeSpan.FromMinutes(30));
            }
        }

        private void PrikaziStranicu()
        {
            if (isLoading)
            {
                loadingText.Visibility = Visibility.Visible;
                return;
            }

            CurrentPagePica.Clear();

            if (cache.TryGetValue("SvaPicaCache", out List<Pice> SvaPicaCache))
            {
                var items = SvaPicaCache.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();

                foreach (var item in items)
                {
                    CurrentPagePica.Add(item);
                }

                loadingText.Visibility = Visibility.Collapsed;
            }
            else
            {
                loadingText.Visibility = Visibility.Visible;
                CurrentPagePica.Clear();
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = (currentPage + 1) * itemsPerPage < (cache.Get<List<Pice>>("SvaPicaCache")?.Count ?? 0);
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
                CurrentPagePica.Clear();
                loadingText.Visibility = Visibility.Visible;
                currentPage++;
            }
        }

        private void Pice_Click(object sender, RoutedEventArgs e)
        {
            SelectedPice = (sender as FrameworkElement)?.DataContext as Pice;

            if (SelectedPice != null)
            {
                UcDetaljiPica detaljiPica = new UcDetaljiPica(SelectedPice, trenutniKorisnik);
                detaljiPica.DataContext = SelectedPice;
                glavniGrid.Children.Clear();
                glavniGrid.Children.Add(detaljiPica);
            }
        }
    }
}
