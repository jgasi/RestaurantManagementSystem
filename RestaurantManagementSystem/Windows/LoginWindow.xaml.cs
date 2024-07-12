using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            var korime = txtKorime.Text;
            var lozinka = txtLozinka.Password;
            
            var pronaden = korisnikServices.GetKorisnikByKorime(korime);

            if (pronaden.Any())
            {
                var korisnik = pronaden.FirstOrDefault();
                if (korisnik != null && korisnik.lozinka == lozinka)
                {
                    //CurrentUser.LoggedInUser = korisnik;

                    if(korisnik.uloga == "Običan korisnik")
                    {
                        MainWindow mainWindow = new MainWindow(korisnik);
                        mainWindow.Show();
                        GuiManager.SetMainWindow(mainWindow);
                        this.Close();
                    }
                    else if(korisnik.uloga == "Administrator")
                    {
                        AdminWindow adminWindow = new AdminWindow(korisnik);
                        adminWindow.Show();
                        GuiManager.SetAdminWindow(adminWindow);
                        this.Close();
                    }
                    else if (korisnik.uloga == "Osoblje")
                    {
                        OsobljeWindow osobljeWindow = new OsobljeWindow(korisnik);
                        osobljeWindow.Show();
                        GuiManager.SetOsobljeWindow(osobljeWindow);
                        this.Close();
                    }
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

        private byte[] LoadImage(string imagePath)
        {
            try
            {
                // Čitanje slike iz datoteke i pretvaranje u byte[] format
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                using (MemoryStream ms = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder(); // Prilagodite kodera ovisno o vrsti slike
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška prilikom učitavanja slike: {ex.Message}");
                return null;
            }
        }
    }
}
