using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcKomentariPica : UserControl
    {
        private readonly RecenzijaServices recenzijaServices = new RecenzijaServices();
        private readonly KorisnikServices korisnikServices = new KorisnikServices();
        private ObservableCollection<Recenzija> comments;
        private static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public UcKomentariPica(Pice selectedPice)
        {
            InitializeComponent();
            comments = new ObservableCollection<Recenzija>();
            commentsListView.ItemsSource = comments;
            LoadComments(selectedPice);
        }

        private async void LoadComments(Pice selectedPice)
        {
            try
            {
                loadingText.Visibility = Visibility.Visible;

                string cacheKey = $"cache_pice_{selectedPice.id_pice}";
                if (!cache.TryGetValue(cacheKey, out ObservableCollection<Recenzija> cachedComments))
                {
                    var recenzije = await recenzijaServices.GetRecenzijePicaByIdAsync(selectedPice.id_pice);
                    cachedComments = new ObservableCollection<Recenzija>();

                    foreach (var recenzija in recenzije)
                    {
                        var korisnik = korisnikServices.GetKorisnikById(recenzija.Korisnik_id_korisnik);
                        recenzija.Korisnik = korisnik;
                        cachedComments.Add(recenzija);
                    }

                    cache.Set(cacheKey, cachedComments, TimeSpan.FromMinutes(30));
                }

                comments = cachedComments;
                commentsListView.ItemsSource = comments;

                loadingText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading comments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
