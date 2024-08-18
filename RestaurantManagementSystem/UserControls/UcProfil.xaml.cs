using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcProfil : UserControl
    {
        public Korisnik TrenutniKorisnik { get; set; }
        private KorisnikServices korisnikServices = new KorisnikServices();
        private bool isImageChanged = false;

        private string globalnaLozinka = "";

        //samo za dodavanje slika za pića
        //private PiceServices piceServices = new PiceServices();

        public UcProfil(Korisnik korisnik)
        {
            InitializeComponent();
            TrenutniKorisnik = korisnik;
            UcitajPodatke();
        }

        public delegate void ProfileImageChangedEventHandler(object sender, EventArgs e);
        public static event ProfileImageChangedEventHandler ProfileImageChanged;

        private void UcitajPodatke()
        {
            if (TrenutniKorisnik != null)
            {
                txtKorime.Text = TrenutniKorisnik.korime;
                txtIme.Text = TrenutniKorisnik.ime;
                txtPrezime.Text = TrenutniKorisnik.prezime;
                txtEmail.Text = TrenutniKorisnik.email;

                if (TrenutniKorisnik.slika != null)
                {
                    imgSlika.Source = ByteToImage(TrenutniKorisnik.slika);
                }
            }
        }

        private void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            string errors = provjeriUnose();
            if (!string.IsNullOrEmpty(errors))
            {
                MessageBox.Show(errors, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TrenutniKorisnik != null)
            {
                TrenutniKorisnik.korime = txtKorime.Text;
                TrenutniKorisnik.lozinka = globalnaLozinka;
                TrenutniKorisnik.ime = txtIme.Text;
                TrenutniKorisnik.prezime = txtPrezime.Text;
                TrenutniKorisnik.email = txtEmail.Text;

                if (isImageChanged)
                {
                    korisnikServices.UpdateKorisnik(TrenutniKorisnik);
                    MessageBox.Show("Podaci su uspješno spremljeni, uključujući sliku!");
                    ProfileImageChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    korisnikServices.UpdateKorisnik(TrenutniKorisnik);
                    MessageBox.Show("Podaci su uspješno spremljeni!");
                }
                isImageChanged = false;
            }
        }


        private string provjeriUnose()
        {
            StringBuilder errorMessage = new StringBuilder();
            string korisnickoIme = txtKorime.Text;
            string lozinka = txtLozinka.Password;
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
            if (txtLozinka.Password != "")
            {
                if (!isLozinkaValid)
                {
                    errorMessage.AppendLine("Lozinka mora imati najmanje 5 znakova, jedan broj i jedno slovo. Može imati i znakove '@$!%*?&'.");
                }
                else
                {
                    globalnaLozinka = korisnikServices.HashPassword(txtLozinka.Password);
                }
            }
            else
            {
                globalnaLozinka = TrenutniKorisnik.lozinka;
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

                TrenutniKorisnik.slika = ImageToByte(bitmap);
                isImageChanged = true;

                //samo za dodavanje slika za pića
                //spremiPice(bitmap);
                
            }
        }

        //samo za dodavanje slika za pića
        //private async void spremiPice(BitmapImage bitmap)
        //{
          //  List<Pice> kola = await piceServices.GetPiceByIdAsync(6);
          //
           // Pice prvoKola = kola.FirstOrDefault();
           // prvoKola.slika = ImageToByte(bitmap);
           // piceServices.UpdatePice(prvoKola);
       // }

        private byte[] ImageToByte(BitmapImage bitmapImage)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder;
                if (bitmapImage.UriSource.AbsolutePath.EndsWith(".png"))
                {
                    encoder = new PngBitmapEncoder();
                }
                else
                {
                    encoder = new JpegBitmapEncoder();
                }

                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private BitmapImage ByteToImage(byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
        }
    }
}
