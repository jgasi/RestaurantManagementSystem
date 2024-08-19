using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Win32;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjePica : UserControl
    {
        private PiceServices piceServices = new PiceServices();
        private InventarServices inventarServices = new InventarServices();
        private Stavka_narudzbeServices stavka_NarudzbeServices = new Stavka_narudzbeServices();
        private NarudzbaServices narudzbaServices = new NarudzbaServices();
        private RezervacijaServices rezervacijaServices = new RezervacijaServices();
        private RecenzijaServices recenzijaServices = new RecenzijaServices();

        private bool isImageChanged = false;
        Pice globalnoPiceEdit;

        public List<Stavka_narudzbe> stavkeZaBrisanje;
        public List<Narudzba> narudzbeZaBrisanje;

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
                loadingText.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void btnNovoPice_Click(object sender, RoutedEventArgs e)
        {
            popupNovoPice.IsOpen = true;
        }

        private void btnSpremi_Click(object sender, RoutedEventArgs e)
        {
            // Provjera da li su sva polja popunjena
            if (string.IsNullOrEmpty(tbNazivPica.Text) || string.IsNullOrEmpty(tbCijena.Text) ||
                string.IsNullOrEmpty(tbNutrivneInformacije.Text) || string.IsNullOrEmpty(tbAlergeni.Text) ||
                string.IsNullOrEmpty(tbInventar.Text))
            {
                MessageWindow messageWindow = new MessageWindow("Molimo vas da popunite sva polja!");
                messageWindow.Top = 0;
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2;
                messageWindow.Show();
                return;
            }

            // Deklaracija varijabli i regex provjere
            StringBuilder errorMessage = new StringBuilder();
            string nazivPica = tbNazivPica.Text.Trim();
            string cijenaPica = tbCijena.Text.Trim();
            string nutritivneInformacije = tbNutrivneInformacije.Text.Trim();
            string alergeni = tbAlergeni.Text.Trim();
            string idInventara = tbInventar.Text.Trim();

            // Validacija pomoću regex-a
            bool isNazivPicaValid = Regex.IsMatch(nazivPica, @"^[a-žA-Ž\s-]+$");
            bool isCijenaPicaValid = Regex.IsMatch(cijenaPica, @"^\d+(\.\d{1,2})?$");
            bool isNutritivneInformacijeValid = Regex.IsMatch(nutritivneInformacije, @"^([a-žA-Ž0-9\s\-\.:]+(,[a-žA-Ž0-9\s\-\.:]+)*)?$");
            bool isAlergeniValid = Regex.IsMatch(alergeni, @"^([a-žA-Ž\s-]+(,[a-žA-Ž\s-]+)*)?$");
            bool isIDInventaraValid = Regex.IsMatch(idInventara, @"^\d+$");
            int parsedId;


            // Provjera grešaka
            if (int.TryParse(idInventara, out parsedId))
            {
                bool doesIDInventaraExist = inventarServices.GetInventarById(int.Parse(parsedId.ToString())) != null;
                if (!doesIDInventaraExist)
                {
                    errorMessage.AppendLine("Upisani ID inventara ne postoji.");
                }
            }
            if (!isIDInventaraValid)
            {
                errorMessage.AppendLine("ID inventara mora biti isključivo broj.");
            }

            if (!isNazivPicaValid)
            {
                errorMessage.AppendLine("Naziv pića može sadržavati samo slova i znak '-'.");
            }
            if (!isCijenaPicaValid)
            {
                errorMessage.AppendLine("Cijena pića mora biti u obliku kao npr:\n     12000.5 ili 12.99.");
            }
            if (!isNutritivneInformacijeValid)
            {
                errorMessage.AppendLine("Nutritivne informacije moraju biti odvojene zarezom te smiju sadržavati samo slova, brojeve te znakove ':', '.' i '-'.\n     Npr: prva: 1.1g, druga-druga: 2.2g, treca: 3.3g");
            }
            if (!isAlergeniValid)
            {
                errorMessage.AppendLine("Alergeni moraju biti odvojeni zarezom te smiju sadržavati samo slova i znak '-'.\n     Npr: prva, druga-druga, treca");
            }

            // Ako postoje greške, prikaži poruku
            if (errorMessage.Length > 0)
            {
                MessageWindow messageWindow = new MessageWindow(errorMessage.ToString());
                messageWindow.Top = 0;
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2;
                messageWindow.Show();
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
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
                            //obrisi recenzije vezane za pice
                            List<Recenzija> vezaneRecenzije = await recenzijaServices.GetRecenzijePicaByIdAsync(pice.id_pice);

                            foreach (var piceZaBrisanje in vezaneRecenzije)
                            {
                                recenzijaServices.RemoveRecenziju(piceZaBrisanje);
                            }

                            // Uklonite sve stavke narudžbi povezane s pićem
                            stavkeZaBrisanje = await stavka_NarudzbeServices.GetAllStavkeNarudzbeByPiceIdAsync(pice.id_pice);

                            // Koristite HashSet za jedinstvene narudžbe
                            HashSet<int> narudzbeZaBrisanjeId = new HashSet<int>();

                            foreach (var stavkaNarudzbe in stavkeZaBrisanje)
                            {
                                int idHelper = stavkaNarudzbe.Narudzba_id_narudzba;
                                narudzbeZaBrisanjeId.Add(idHelper);
                            }

                            // Dohvatite jedinstvene narudžbe iz baze
                            List<Narudzba> narudzbeZaBrisanjeLista = new List<Narudzba>();
                            foreach (var narudzbaId in narudzbeZaBrisanjeId)
                            {
                                var narudzba = await narudzbaServices.GetNarudzbuByIdAsync(narudzbaId);
                                if (narudzba != null)
                                {
                                    narudzbeZaBrisanjeLista.Add(narudzba);
                                }
                            }

                            // Dalje brisanje stavki i narudžbi kao prije
                            foreach (var narudzbaObrisi in narudzbeZaBrisanjeLista)
                            {
                                List<Stavka_narudzbe> stavkeHelper = await stavka_NarudzbeServices.GetAllStavkeNarudzbeByNarudzbaIdAsync(narudzbaObrisi.id_narudzba);

                                foreach (var obrisiPreostale in stavkeHelper)
                                {
                                    stavka_NarudzbeServices.RemoveStavkeNarudzbe(obrisiPreostale);
                                }

                                narudzbaServices.RemoveNarudzbu(narudzbaObrisi);
                            }

                            piceServices.RemovePice(pice);
                            loadingText.Visibility = Visibility.Visible;
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
            // Provjera da li su sva polja popunjena
            if (string.IsNullOrEmpty(tbNazivPicaUredi.Text) || string.IsNullOrEmpty(tbCijenaUredi.Text) ||
                string.IsNullOrEmpty(tbNutrivneInformacijeUredi.Text) || string.IsNullOrEmpty(tbAlergeniUredi.Text) ||
                string.IsNullOrEmpty(tbInventarUredi.Text))
            {
                MessageWindow messageWindow = new MessageWindow("Molimo vas da popunite sva polja!");
                messageWindow.Top = 0;
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2;
                messageWindow.Show();
                return;
            }

            // Deklaracija varijabli i regex provjere
            StringBuilder errorMessage = new StringBuilder();
            string nazivPica = tbNazivPicaUredi.Text.Trim();
            string cijenaPica = tbCijenaUredi.Text.Trim();
            string nutritivneInformacije = tbNutrivneInformacijeUredi.Text.Trim();
            string alergeni = tbAlergeniUredi.Text.Trim();
            string idInventara = tbInventarUredi.Text.Trim();

            // Validacija pomoću regex-a
            bool isNazivPicaValid = Regex.IsMatch(nazivPica, @"^[a-žA-Ž\s-]+$");
            bool isCijenaPicaValid = Regex.IsMatch(cijenaPica, @"^\d+(\.\d{1,2})?$");
            bool isNutritivneInformacijeValid = Regex.IsMatch(nutritivneInformacije, @"^([a-žA-Ž0-9\s\-\.:]+(,[a-žA-Ž0-9\s\-\.:]+)*)?$");
            bool isAlergeniValid = Regex.IsMatch(alergeni, @"^([a-žA-Ž\s-]+(,[a-žA-Ž\s-]+)*)?$");
            bool isIDInventaraValid = Regex.IsMatch(idInventara, @"^\d+$");
            int parsedId;


            // Provjera grešaka
            if (int.TryParse(idInventara, out parsedId))
            {
                bool doesIDInventaraExist = inventarServices.GetInventarById(int.Parse(parsedId.ToString())) != null;
                if (!doesIDInventaraExist)
                {
                    errorMessage.AppendLine("Upisani ID inventara ne postoji.");
                }
            }
            if (!isIDInventaraValid)
            {
                errorMessage.AppendLine("ID inventara mora biti isključivo broj.");
            }

            if (!isNazivPicaValid)
            {
                errorMessage.AppendLine("Naziv pića može sadržavati samo slova i znak '-'.");
            }
            if (!isCijenaPicaValid)
            {
                errorMessage.AppendLine("Cijena pića mora biti u obliku kao npr:\n     12000.5 ili 12.99.");
            }
            if (!isNutritivneInformacijeValid)
            {
                errorMessage.AppendLine("Nutritivne informacije moraju biti odvojene zarezom te smiju sadržavati samo slova, brojeve te znakove ':', '.' i '-'.\n     Npr: prva: 1.1g, druga-druga: 2.2g, treca: 3.3g");
            }
            if (!isAlergeniValid)
            {
                errorMessage.AppendLine("Alergeni moraju biti odvojeni zarezom te smiju sadržavati samo slova i znak '-'.\n     Npr: prva, druga-druga, treca");
            }


            // Ako postoje greške, prikaži poruku
            if (errorMessage.Length > 0)
            {
                MessageWindow messageWindow = new MessageWindow(errorMessage.ToString());
                messageWindow.Top = 0;
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2;
                messageWindow.Show();
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
