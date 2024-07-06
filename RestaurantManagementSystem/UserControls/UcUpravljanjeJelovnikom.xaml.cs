using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjeJelovnikom : UserControl
    {
        private JeloServices jeloServices = new JeloServices();
        private bool isImageChanged = false;
        Jelo globalnoJeloEdit;

        public UcUpravljanjeJelovnikom()
        {
            InitializeComponent();
            loadingText.Visibility = Visibility.Visible;
            UcitajJela();
            tbSearch.TextChanged += TbSearch_TextChanged;
        }

        private async void UcitajJela()
        {
            try
            {
                var jela = await jeloServices.GetAllJelaAsync();

                foreach (var jelo in jela)
                {
                    jelo.nutrivne_informacije = string.Join("\n", jelo.nutrivne_informacije.Split(','));
                    jelo.alergeni = string.Join("\n", jelo.alergeni.Split(','));
                }

                dgJela.ItemsSource = jela;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška prilikom učitavanja jela: " + ex.Message);
            }
            finally
            {
                loadingText.Visibility = Visibility.Collapsed;
            }
        }

        private void btnNovoJelo_Click(object sender, RoutedEventArgs e)
        {
            popupNovoJelo.IsOpen = true;
        }

        private void btnSpremi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbNazivJela.Text) || string.IsNullOrEmpty(tbCijena.Text) || string.IsNullOrEmpty(tbNutrivneInformacije.Text) || string.IsNullOrEmpty(tbAlergeni.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja!");
                return;
            }

            Jelo novoJelo = new Jelo
            {
                naziv = tbNazivJela.Text,
                cijena = tbCijena.Text,
                nutrivne_informacije = tbNutrivneInformacije.Text,
                alergeni = tbAlergeni.Text,
                Inventar_id_inventar = 1
            };

            if (isImageChanged)
            {
                BitmapImage bitmap = imgSlika.Source as BitmapImage;
                if (bitmap != null)
                {
                    novoJelo.slika = ImageToByte(bitmap);
                }
            }

            jeloServices.AddJelo(novoJelo);
            popupNovoJelo.IsOpen = false;
            loadingText.Visibility = Visibility.Visible;
            UcitajJela();
        }

        private void btnZatvori_Click(object sender, RoutedEventArgs e)
        {
            popupNovoJelo.IsOpen = false;
        }

        private void btnOdaberiSliku_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                bitmap.EndInit();
                imgSlika.Source = bitmap;
                isImageChanged = true;
            }
        }

        private byte[] ImageToByte(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Jelo jelo = button.DataContext as Jelo;
                if (jelo != null)
                {
                    MessageBoxResult result = MessageBox.Show("Jeste li sigurni da želite obrisati ovo jelo?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            jeloServices.RemoveJelo(jelo);
                            UcitajJela();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Greška prilikom brisanja jela: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Jelo nije moguće obrisati.");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            globalnoJeloEdit = button.DataContext as Jelo;

            popupUrediJelo.IsOpen = true;

            tbNazivJelaUredi.Text = globalnoJeloEdit.naziv;
            tbCijenaUredi.Text = globalnoJeloEdit.cijena;
            tbNutrivneInformacijeUredi.Text = RestoreNutritionalInfoFormat(globalnoJeloEdit.nutrivne_informacije);
            tbAlergeniUredi.Text = RestoreAllergensFormat(globalnoJeloEdit.alergeni);

            if (globalnoJeloEdit.slika != null && globalnoJeloEdit.slika.Length > 0)
            {
                BitmapImage bitmapImage = ByteToImage(globalnoJeloEdit.slika);
                imgSlikaUredi.Source = bitmapImage;
            }
            else
            {
                imgSlikaUredi.Source = null;
            }
        }

        private string RestoreNutritionalInfoFormat(string formattedString)
        {
            if (string.IsNullOrEmpty(formattedString))
                return string.Empty;

            return formattedString.Replace("\n", ",");
        }

        private string RestoreAllergensFormat(string formattedString)
        {
            if (string.IsNullOrEmpty(formattedString))
                return string.Empty;

            return formattedString.Replace("\n", ",");
        }


        private BitmapImage ByteToImage(byte[] imageData)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void btnOdaberiSlikuUredi_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                bitmap.EndInit();
                imgSlikaUredi.Source = bitmap;
                isImageChanged = true;
            }
        }


        private void btnSpremiUredi_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(tbNazivJelaUredi.Text) || string.IsNullOrEmpty(tbCijenaUredi.Text) || string.IsNullOrEmpty(tbNutrivneInformacijeUredi.Text) || string.IsNullOrEmpty(tbAlergeniUredi.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja!");
                return;
            }

            globalnoJeloEdit.naziv = tbNazivJelaUredi.Text;
            globalnoJeloEdit.cijena = tbCijenaUredi.Text;
            globalnoJeloEdit.nutrivne_informacije = tbNutrivneInformacijeUredi.Text;
            globalnoJeloEdit.alergeni = tbAlergeniUredi.Text;
            globalnoJeloEdit.Inventar_id_inventar = 1;

            if (isImageChanged)
            {
                BitmapImage bitmap = imgSlikaUredi.Source as BitmapImage;
                if (bitmap != null)
                {
                    globalnoJeloEdit.slika = ImageToByte(bitmap);
                }
            }

            jeloServices.UpdateJelo(globalnoJeloEdit);
            popupUrediJelo.IsOpen = false;
            loadingText.Visibility = Visibility.Visible;
            UcitajJela();
        }

        private void btnZatvoriUredi_Click(object sender, RoutedEventArgs e)
        {
            popupUrediJelo.IsOpen = false;
        }

        private async void btnSearchJelo_Click(object sender, RoutedEventArgs e)
        {
            var searchTekst = tbSearch.Text;
            var searchedJela = await jeloServices.GetJelaByNameAsync(searchTekst);

            foreach (var jelo in searchedJela)
            {
                jelo.nutrivne_informacije = string.Join("\n", jelo.nutrivne_informacije.Split(','));
                jelo.alergeni = string.Join("\n", jelo.alergeni.Split(','));
            }
            dgJela.ItemsSource = searchedJela;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTekst = tbSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTekst))
            {
                UcitajJela();
            }
        }
    }
}
