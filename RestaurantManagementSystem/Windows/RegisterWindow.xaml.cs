using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private KorisnikServices korisnikServices = new KorisnikServices();


        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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

        private void TextBlockLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void TextBlockInformacije_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            //tu napisi sta hoces za informacije
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            //Ako budem htio dodat textblockove za greske
            ResetErrorMessages();

            string errors = provjeriUnose();
            if (string.IsNullOrEmpty(errors))
            {
                Korisnik korisnik = new Korisnik
                {
                    korime = txtKorime.Text,
                    lozinka = txtLozinka.Text,
                    ime = txtIme.Text,
                    prezime = txtPrezime.Text,
                    email = txtEmail.Text,
                    uloga = "Običan korisnik"
                };

                korisnikServices.AddKorisnik(korisnik);
                MessageBox.Show("Registracija uspješna!", "Obavijest", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(errors, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string provjeriUnose()
        {
            StringBuilder errorMessage = new StringBuilder();
            string korisnickoIme = txtKorime.Text;
            string lozinka = txtLozinka.Text;
            string ime = txtIme.Text;
            string prezime = txtPrezime.Text;
            string email = txtEmail.Text;

            bool isKorisnickoImeValid = Regex.IsMatch(korisnickoIme, @"^[a-zA-Z0-9_]{3,20}$");
            bool isLozinkaValid = Regex.IsMatch(lozinka, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{5,}$");
            bool isImeValid = Regex.IsMatch(ime, @"^[a-zA-Z\s-]{1,}$");
            bool isPrezimeValid = Regex.IsMatch(prezime, @"^[a-zA-Z\s-]{1,}$");
            bool isEmailValid = Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (!isKorisnickoImeValid)
            {
                errorMessage.AppendLine("Korisničko ime mora imati između 3 i 20 znakova, i može sadržavati slova, brojeve i donje crte.");
            }
            if (!isLozinkaValid)
            {
                errorMessage.AppendLine("Lozinka mora imati najmanje 5 znakova, jedan broj i jedno slovo. Može imati i znakove '@$!%*?&'.");
            }
            if (!isImeValid)
            {
                errorMessage.AppendLine("Ime može sadržavati samo slova, znak '-' i razmake.");
            }
            if (!isPrezimeValid)
            {
                errorMessage.AppendLine("Prezime može sadržavati samo slova, znak '-' i razmake.");
            }
            if (!isEmailValid)
            {
                errorMessage.AppendLine("Email adresa nije u ispravnom formatu.\n     - Primjer maila: ime.prezime5@gmail.com");
            }

            return errorMessage.ToString();
        }


        //Ako budem htio dodat textblockove za greske
        private void ResetErrorMessages()
        {
            txtKorimeError.Visibility = Visibility.Collapsed;
            txtLozinkaError.Visibility = Visibility.Collapsed;
            txtImeError.Visibility = Visibility.Collapsed;
            txtPrezimeError.Visibility = Visibility.Collapsed;
            txtEmailError.Visibility = Visibility.Collapsed;
        }

    }
}
