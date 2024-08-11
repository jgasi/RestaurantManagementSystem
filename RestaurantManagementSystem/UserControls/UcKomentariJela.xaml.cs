using System;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcKomentariJela : UserControl
    {
        private readonly RecenzijaServices recenzijaServices = new RecenzijaServices();
        private readonly KorisnikServices korisnikServices = new KorisnikServices();
        private ObservableCollection<Recenzija> comments;

        public UcKomentariJela(Jelo selectedJelo)
        {
            InitializeComponent();
            comments = new ObservableCollection<Recenzija>();
            commentsListView.ItemsSource = comments;
            LoadComments(selectedJelo);
        }

        private async void LoadComments(Jelo selectedJelo)
        {
            try
            {
                loadingText.Visibility = Visibility.Visible;

                var recenzije = await recenzijaServices.GetRecenzijeByIdAsync(selectedJelo.id_jelo);

                // Očistite postojeće komentare prije dodavanja novih
                comments.Clear();

                foreach (var recenzija in recenzije)
                {
                    var korisnik = korisnikServices.GetKorisnikById(recenzija.Korisnik_id_korisnik);
                    recenzija.Korisnik = korisnik;
                    comments.Add(recenzija);
                }

                loadingText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading comments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteComment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                var comment = textBlock.DataContext as Recenzija;
                if (comment != null)
                {
                    MessageBoxResult result = MessageBox.Show("Želite li stvarno obrisati ovaj komentar?", "Potvrda brisanja", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            recenzijaServices.RemoveRecenziju(comment);
                            comments.Remove(comment);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Greška prilikom brisanja: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }
}
