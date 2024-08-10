using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjeKorisnicima : UserControl
    {
        private KorisnikServices korisnikServices = new KorisnikServices();
        private Stavka_narudzbeServices stavka_narudzbeServices = new Stavka_narudzbeServices();
        private NarudzbaServices narudzbaServices = new NarudzbaServices();
        private RecenzijaServices recenzijaServices = new RecenzijaServices();
        private RezervacijaServices rezervacijaServices = new RezervacijaServices();

        public List<Narudzba> sveNarudzbeKorisnika;
        public List<Recenzija> sveRecenzijeKorisnika;
        public List<Rezervacija> sveRezervacijeKorisnika;

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
                    if(korisnik.uloga != "Administrator") 
                    {
                        MessageBoxResult result = MessageBox.Show(
                        "Jeste li sigurni da želite obrisati korisnika " + korisnik.korime + "?",
                        "Potvrda brisanja",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                        if (result == MessageBoxResult.Yes)
                        {
                            ObrisiKorisnikaIVeze(korisnik);

                            //korisnikServices.RemoveKorisnik(korisnik);
                            LoadUsers();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Administratora nije moguće obrisati.");
                    }
                }
            }
        }

        private async void ObrisiKorisnikaIVeze(Korisnik korisnik)
        {
            //obrisi stavke narudzbe od svih korisnikovih narudzbi
            sveNarudzbeKorisnika = await narudzbaServices.GetAllNarudzbeByKorisnikAsync(korisnik.id_korisnik);

            foreach (Narudzba narudzba in sveNarudzbeKorisnika)
            {
                await stavka_narudzbeServices.RemoveStavkeNarudzbeByIdAsync(narudzba.id_narudzba);
            }

            //obrisi sve narudzbe korisnika
            foreach (Narudzba narudzba in sveNarudzbeKorisnika)
            {
                narudzbaServices.RemoveNarudzbu(narudzba);
            }

            //obrisi sve recenzije korisnika
            sveRecenzijeKorisnika = await recenzijaServices.GetRecenzijeByKorisnikIdAsync(korisnik.id_korisnik);

            foreach (Recenzija recenzija in sveRecenzijeKorisnika)
            {
                recenzijaServices.RemoveRecenziju(recenzija);
            }

            //obrisi sve rezervacije korisnika
            sveRezervacijeKorisnika = await rezervacijaServices.GetAllRezervacijeByKorisnikId(korisnik.id_korisnik);
            foreach (Rezervacija rezervacija in sveRezervacijeKorisnika)
            {
                rezervacijaServices.RemoveRezervaciju(rezervacija);
            }

            //obrisi korisnika
            korisnikServices.RemoveKorisnik(korisnik);

            LoadUsers();
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
