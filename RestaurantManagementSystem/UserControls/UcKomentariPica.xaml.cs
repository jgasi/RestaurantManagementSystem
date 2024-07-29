using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcKomentariPica : UserControl
    {
        private readonly RecenzijaServices recenzijaServices = new RecenzijaServices();
        private readonly KorisnikServices korisnikServices = new KorisnikServices();
        private ObservableCollection<Recenzija> comments;

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

                var recenzije = await recenzijaServices.GetRecenzijePicaByIdAsync(selectedPice.id_pice);
                comments.Clear(); // Očisti postojeće komentare

                foreach (var recenzija in recenzije)
                {
                    var korisnik = korisnikServices.GetKorisnikById(recenzija.Korisnik_id_korisnik);
                    recenzija.Korisnik = korisnik;
                    comments.Add(recenzija);
                }

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
