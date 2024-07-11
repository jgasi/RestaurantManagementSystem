using System;
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

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcDetaljiPica : UserControl
    {
        private PiceServices piceServices = new PiceServices();
        private RecenzijaServices recenzijaServices = new RecenzijaServices();
        private Korisnik trenutniKorisnik;

        public Pice primljenoPice;
        public UcDetaljiPica(Pice selectedPice, Korisnik korisnik)
        {
            InitializeComponent();
            DataContext = selectedPice;
            primljenoPice = selectedPice;
            PrikaziDetalje(primljenoPice);
            trenutniKorisnik = korisnik;
        }

        public void PrikaziDetalje(Pice pice)
        {
            nazivTxtBl.Text = pice.naziv;
            cijenaTxtBl.Text = pice.cijena;
            slikaPica.Source = ByteToImage(pice.slika);

            nutrivneInformacijeStackPanel.Children.Clear();
            string[] nutrivneInformacije = pice.nutrivne_informacije.Split(',');
            foreach (var info in nutrivneInformacije)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = info,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 2, 0, 2),
                    FontSize = 14,
                    FontFamily = (FontFamily)FindResource("CustomFont2")
                };
                nutrivneInformacijeStackPanel.Children.Add(textBlock);
            }

            alergeniStackPanel.Children.Clear();
            string[] alergeni = pice.alergeni.Split(',');
            foreach (var alergen in alergeni)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = alergen,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 2, 0, 2),
                    FontSize = 14,
                    FontFamily = (FontFamily)FindResource("CustomFont2")

                };
                alergeniStackPanel.Children.Add(textBlock);
            }
        }

        private void CommentsButton_Click(object sender, RoutedEventArgs e)
        {
            UcKomentariPica ucKomentariPica = new UcKomentariPica(primljenoPice);
            glavniGrid.Children.Clear();
            glavniGrid.Children.Add(ucKomentariPica);
        }

        private BitmapImage ByteToImage(byte[] imageData)
        {
            if (imageData == null)
            {
                System.Windows.Forms.MessageBox.Show("Slika je null!");
                return null;
            }
            else
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

        byte[] ConvertImageToBytes(ImageSource imageSource)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private void DodajKomentarButton_Click(object sender, RoutedEventArgs e)
        {
            popupKomentari.IsOpen = true;
        }

        private void Odustani_Click(object sender, RoutedEventArgs e)
        {
            popupKomentari.IsOpen = false;
        }

        private void SpremiKomentar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBoxKomentar.Text))
            {
                MessageBox.Show("Molimo unesite tekst komentara.");
                return;
            }

            int odabranaOcjena = int.Parse((ocjenaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());

            Recenzija noviKomentar = new Recenzija
            {
                ocjena = odabranaOcjena.ToString(),
                komentar = txtBoxKomentar.Text,
                Korisnik_id_korisnik = trenutniKorisnik.id_korisnik,
                Pice_id_pice = primljenoPice.id_pice
            };

            bool uspjesnoDodan = recenzijaServices.AddRecenziju(noviKomentar);

            if (uspjesnoDodan)
            {
                MessageBox.Show("Komentar uspješno dodan.");
                popupKomentari.IsOpen = false;
            }
            else
            {
                MessageBox.Show("Došlo je do greške prilikom dodavanja komentara.");
            }
        }
    }
}
