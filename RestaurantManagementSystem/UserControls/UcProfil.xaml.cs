using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using RestaurantManagementSystem.Autentikacija;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcProfil : UserControl
    {
        public Korisnik TrenutniKorisnik { get; set; }
        private KorisnikServices korisnikServices = new KorisnikServices();
        private bool isImageChanged = false;

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
                txtLozinka.Text = TrenutniKorisnik.lozinka;
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
            if (TrenutniKorisnik != null)
            {
                TrenutniKorisnik.korime = txtKorime.Text;
                TrenutniKorisnik.lozinka = txtLozinka.Text;
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
