﻿using System;
using System.Collections.Generic;
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
        private bool isImageChanged = false;


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
            }
        }

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
