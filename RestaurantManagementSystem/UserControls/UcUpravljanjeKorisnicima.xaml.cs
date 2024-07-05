using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjeKorisnicima : UserControl
    {
        private KorisnikServices korisnikServices = new KorisnikServices();

        public UcUpravljanjeKorisnicima()
        {
            InitializeComponent();
            loadingText.Visibility = Visibility.Visible;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                dgKorisnici.ItemsSource = await korisnikServices.GetAllKorisnikeAsync();

                loadingText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Korisnik korisnik = button.DataContext as Korisnik;
                if (korisnik != null)
                {
                    if (korisnik.uloga != "Administrator")
                    {
                        korisnikServices.RemoveKorisnik(korisnik);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Administratora nije moguće obrisati.");
                    }
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                Korisnik korisnik = dgKorisnici.SelectedItem as Korisnik;
                if (korisnik != null)
                {
                    korisnik.uloga = radioButton.Content.ToString();
                    korisnikServices.UpdateKorisnik(korisnik);
                }
                LoadUsers();
            }
        }
    }
}
