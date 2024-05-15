using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using RestaurantManagementSystem.Autentikacija;

namespace RestaurantManagementSystem.UserControls
{
    /// <summary>
    /// Interaction logic for UcProfil.xaml
    /// </summary>
    public partial class UcProfil : UserControl
    {
        public Korisnik TrenutniKorisnik { get; set; }
        private KorisnikServices korisnikServices = new KorisnikServices();

        public UcProfil()
        {
            InitializeComponent();
            UcitajPodatke();
        }

        private void UcitajPodatke()
        {
            TrenutniKorisnik = CurrentUser.LoggedInUser;

            if (TrenutniKorisnik != null)
            {
                txtKorime.Text = TrenutniKorisnik.korime;
                txtLozinka.Text = TrenutniKorisnik.lozinka;
                txtIme.Text = TrenutniKorisnik.ime;
                txtPrezime.Text = TrenutniKorisnik.prezime;
                txtEmail.Text = TrenutniKorisnik.email;
            }
        }

        private void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            if (TrenutniKorisnik != null)
            {
                TrenutniKorisnik.korime = txtKorime.Text;
                TrenutniKorisnik.lozinka = txtLozinka.Text;
                TrenutniKorisnik.ime = txtIme.Text;
                TrenutniKorisnik.prezime = txtPrezime.Text;
                TrenutniKorisnik.email = txtEmail.Text;
                // treba dodati jos za sliku

                korisnikServices.UpdateKorisnik(TrenutniKorisnik);
                MessageBox.Show("Podaci su uspješno spremljeni!");
            }
        }
        private void BtnPromijeniSliku_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // Prikaz nove slike
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                bitmap.EndInit();
                imgSlika.Source = bitmap;
            }
        }
    }
}
