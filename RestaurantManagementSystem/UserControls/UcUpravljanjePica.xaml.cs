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
    public partial class UcUpravljanjePica : UserControl
    {
        private PiceServices piceServices = new PiceServices();
        private bool isImageChanged = false;
        Pice globalnoPiceEdit;

        public UcUpravljanjePica()
        {
            InitializeComponent();
            loadingText.Visibility = Visibility.Visible;
            UcitajPica();
            tbSearch.TextChanged += TbSearch_TextChanged;
        }

        private async void UcitajPica()
        {
            try
            {
                var pica = await piceServices.GetAllPicaAsync();

                foreach (var pice in pica)
                {
                    pice.nutrivne_informacije = string.Join("\n", pice.nutrivne_informacije.Split(','));
                    pice.alergeni = string.Join("\n", pice.alergeni.Split(','));
                }

                dgPica.ItemsSource = pica;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška prilikom učitavanja pića: " + ex.Message);
            }
            finally
            {
                loadingText.Visibility = Visibility.Collapsed;
            }
        }

        private void btnNovoPice_Click(object sender, RoutedEventArgs e)
        {
            popupNovoPice.IsOpen = true;
        }

        private void btnSpremi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbNazivPica.Text) || string.IsNullOrEmpty(tbCijena.Text) || string.IsNullOrEmpty(tbNutrivneInformacije.Text) || string.IsNullOrEmpty(tbAlergeni.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja!");
                return;
            }

            Pice novoPice = new Pice
            {
                naziv = tbNazivPica.Text,
                cijena = tbCijena.Text,
                nutrivne_informacije = tbNutrivneInformacije.Text,
                alergeni = tbAlergeni.Text,
                Inventar_id_inventar = int.Parse(tbInventar.Text)
            };

            if (isImageChanged)
            {
                BitmapImage bitmap = imgSlika.Source as BitmapImage;
                if (bitmap != null)
                {
                    novoPice.slika = ImageToByte(bitmap);
                }
            }

            piceServices.AddPice(novoPice);
            popupNovoPice.IsOpen = false;
            loadingText.Visibility = Visibility.Visible;
            UcitajPica();
        }

        private void btnZatvori_Click(object sender, RoutedEventArgs e)
        {
            popupNovoPice.IsOpen = false;
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
                Pice pice = button.DataContext as Pice;
                if (pice != null)
                {
                    MessageBoxResult result = MessageBox.Show("Jeste li sigurni da želite obrisati ovo piće?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            piceServices.RemovePice(pice);
                            UcitajPica();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Greška prilikom brisanja pića: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Piće nije moguće obrisati.");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            globalnoPiceEdit = button.DataContext as Pice;

            popupUrediPice.IsOpen = true;

            tbNazivPicaUredi.Text = globalnoPiceEdit.naziv;
            tbCijenaUredi.Text = globalnoPiceEdit.cijena;
            tbNutrivneInformacijeUredi.Text = RestoreNutritionalInfoFormat(globalnoPiceEdit.nutrivne_informacije);
            tbAlergeniUredi.Text = RestoreAllergensFormat(globalnoPiceEdit.alergeni);
            tbInventarUredi.Text = globalnoPiceEdit.Inventar_id_inventar.ToString();

            if (globalnoPiceEdit.slika != null && globalnoPiceEdit.slika.Length > 0)
            {
                BitmapImage bitmapImage = ByteToImage(globalnoPiceEdit.slika);
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
            if (string.IsNullOrEmpty(tbNazivPicaUredi.Text) || string.IsNullOrEmpty(tbCijenaUredi.Text) || string.IsNullOrEmpty(tbNutrivneInformacijeUredi.Text) || string.IsNullOrEmpty(tbAlergeniUredi.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja!");
                return;
            }

            globalnoPiceEdit.naziv = tbNazivPicaUredi.Text;
            globalnoPiceEdit.cijena = tbCijenaUredi.Text;
            globalnoPiceEdit.nutrivne_informacije = tbNutrivneInformacijeUredi.Text;
            globalnoPiceEdit.alergeni = tbAlergeniUredi.Text;
            globalnoPiceEdit.Inventar_id_inventar = int.Parse(tbInventarUredi.Text);

            if (isImageChanged)
            {
                BitmapImage bitmap = imgSlikaUredi.Source as BitmapImage;
                if (bitmap != null)
                {
                    globalnoPiceEdit.slika = ImageToByte(bitmap);
                }
            }

            piceServices.UpdatePice(globalnoPiceEdit);
            popupUrediPice.IsOpen = false;
            loadingText.Visibility = Visibility.Visible;
            UcitajPica();
        }

        private void btnZatvoriUredi_Click(object sender, RoutedEventArgs e)
        {
            popupUrediPice.IsOpen = false;
        }

        private async void btnSearchPice_Click(object sender, RoutedEventArgs e)
        {
            var searchTekst = tbSearch.Text;
            var searchedPica = await piceServices.GetAllPicaByNameAsync(searchTekst);

            foreach (var pice in searchedPica)
            {
                pice.nutrivne_informacije = string.Join("\n", pice.nutrivne_informacije.Split(','));
                pice.alergeni = string.Join("\n", pice.alergeni.Split(','));
            }
            dgPica.ItemsSource = searchedPica;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTekst = tbSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTekst))
            {
                UcitajPica();
            }
        }
    }
}
