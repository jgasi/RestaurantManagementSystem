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

        private void CommentsButton_Click(object sender, RoutedEventArgs e)
        {
            UcKomentariJela ucKomentariJela = new UcKomentariJela(primljenoJelo);
            glavniGrid.Children.Clear();
            glavniGrid.Children.Add(ucKomentariJela);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(primljenoJelo.naziv);
        }
    }
}
