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
    /// <summary>
    /// Interaction logic for UcDetaljiJela.xaml
    /// </summary>
    public partial class UcDetaljiJela : UserControl
    {
        private JeloServices jeloServices = new JeloServices();

        public Jelo primljenoJelo;
        public UcDetaljiJela(Jelo selectedJelo)
        {
            InitializeComponent();
            DataContext = selectedJelo;
            primljenoJelo = selectedJelo;
            PrikaziDetalje(primljenoJelo);
        }

        public void PrikaziDetalje(Jelo jelo)
        {
            nazivTxtBl.Text = jelo.naziv;
            cijenaTxtBl.Text = jelo.cijena;
            slikaJela.Source = ByteToImage(jelo.slika);
            if(jelo.slika == null)
            {
                //ovdje samo ime slike promjeni i idi dalje sejvaj
                var slikaa = slikaJela.Source = new BitmapImage(new Uri("/TempSlike/pileci-sa-zara-i-riza.jpg", UriKind.Relative));

                Jelo updejt = new Jelo
                {
                    //PROMJENI ID JELA KOJEG MIJENJAS BTW POGLEDAJ NA AZURE KOJI JE ID ZA KOJE JELO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    id_jelo = 12,
                    naziv = jelo.naziv,
                    cijena = jelo.cijena,
                    nutrivne_informacije = jelo.nutrivne_informacije,
                    alergeni = jelo.alergeni,
                    slika = ConvertImageToBytes(slikaa),
                    Inventar_id_inventar = 1
                };

                jeloServices.UpdateJelo(updejt);
            }

            nutrivneInformacijeStackPanel.Children.Clear();
            string[] nutrivneInformacije = jelo.nutrivne_informacije.Split('|');
            foreach (var info in nutrivneInformacije)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = info,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 2, 0, 2),
                    FontSize = 16,
                    FontFamily = (FontFamily)FindResource("CustomFont2")
                };
                nutrivneInformacijeStackPanel.Children.Add(textBlock);
            }

            alergeniStackPanel.Children.Clear();
            string[] alergeni = jelo.alergeni.Split('|');
            foreach (var alergen in alergeni)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = alergen,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 2, 0, 2),
                    FontSize = 16,
                    FontFamily = (FontFamily)FindResource("CustomFont2")

                };
                alergeniStackPanel.Children.Add(textBlock);
            }
        }

        private BitmapImage ByteToImage(byte[] imageData)
        {
            if(imageData == null)
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

        // UcDetaljiJela.xaml.cs

        private void Povratak_Click(object sender, RoutedEventArgs e)
        {
            // Kada se pritisne tipka "povratak", vratite se na UcPregledJelovnika
            UcPregledJelovnika pregledJelovnika = new UcPregledJelovnika();
            glavniGrid.Children.Clear();
            glavniGrid.Children.Add(pregledJelovnika);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(primljenoJelo.naziv);
        }
    }
}
