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
using System.Windows.Shapes;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using RestaurantManagementSystem.Autentikacija;

namespace RestaurantManagementSystem
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        private KorisnikServices korisnikServices = new KorisnikServices();
        private JeloServices jeloServices = new JeloServices();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            textBlock.TextDecorations = null;
        }

        private void TextBlockRegistracija_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void TextBlockInformacije_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            //tu napisi sta hoces za informacije
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //sam malo
            //var jelo = new Jelo
            //{
            //    naziv = "Pileći file sa žara i riža",
            //    cijena = "10.99",
            //    nutrivne_informacije = "Kalorije: 295 kcal|Proteini: 33.4g|Ugljikohidrati: 28g|Masti: 3.9g|Zasićene masti: 1.1g|Vlakna: 0.4g|Šećeri: 0g|Natrij: 75mg|Kalcij: 21mg|Željezo: 2.1mg|Kalij: 291mg|Vitamin A: 13 IU|Vitamin C: 0mg|Vitamin D: 1 IU|Vitamin B6: 0.6mg|Vitamin B12: 0.3µg",
            //    alergeni = "Pšenica|Jaja|Riba|Kikiriki|Soja|Mlijeko|Orašasti plodovi|Celer|Gorušica|Sjeme sezama|Sumporni dioksid i sulfiti|Lupin|Mekušci",
             //   Inventar_id_inventar = 1
            //};
            
           // jeloServices.AddJelo(jelo);



            var korime = txtKorime.Text;
            var lozinka = txtLozinka.Password;
            
            var pronaden = korisnikServices.GetKorisnikByKorime(korime);

            if (pronaden.Any())
            {
                var korisnik = pronaden.FirstOrDefault();
                if (korisnik != null && korisnik.lozinka == lozinka)
                {
                    CurrentUser.LoggedInUser = korisnik;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Korisničko ime ili lozinka nisu točni!");
                } 
            }
            else
            {
                MessageBox.Show("Korisničko ime ili lozinka nisu točni!");
            }
        }
    }
}
