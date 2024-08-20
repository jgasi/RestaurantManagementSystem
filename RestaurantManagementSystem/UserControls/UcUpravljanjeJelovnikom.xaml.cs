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
    public partial class UcUpravljanjeJelovnikom : UserControl
    {
        private JeloServices jeloServices = new JeloServices();
        private PiceServices piceServices = new PiceServices();
        private InventarServices inventarServices = new InventarServices();
        private Stavka_narudzbeServices stavka_NarudzbeServices = new Stavka_narudzbeServices();
        private NarudzbaServices narudzbaServices = new NarudzbaServices();
        private RezervacijaServices rezervacijaServices = new RezervacijaServices();
        private RecenzijaServices recenzijaServices = new RecenzijaServices();

        private bool isImageChanged = false;
        Jelo globalnoJeloEdit;

        public List<Stavka_narudzbe> stavkeZaBrisanje;
        public List<Narudzba> narudzbeZaBrisanje;

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
                loadingText.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void btnNovoJelo_Click(object sender, RoutedEventArgs e)
        {
            popupNovoJelo.IsOpen = true;
        }

        private void btnSpremi_Click(object sender, RoutedEventArgs e)
        {
            // Provjera da li su sva polja popunjena
            if (string.IsNullOrEmpty(tbNazivJela.Text) || string.IsNullOrEmpty(tbCijena.Text) ||
                string.IsNullOrEmpty(tbNutrivneInformacije.Text) || string.IsNullOrEmpty(tbAlergeni.Text) ||
                string.IsNullOrEmpty(tbInventar.Text))
            {
                MessageWindow messageWindow = new MessageWindow("Molimo vas da popunite sva polja!");
                messageWindow.Top = 0; // Postavi Y koordinatu na vrh ekrana
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2; // Centriraj X koordinatu
                messageWindow.Show();
                return;
            }

            // Deklaracija varijabli i regex provjere
            StringBuilder errorMessage = new StringBuilder();
            string nazivJela = tbNazivJela.Text;
            string cijenaJela = tbCijena.Text;
            string nutritivneInformacije = tbNutrivneInformacije.Text;
            string alergeni = tbAlergeni.Text;
            string idInventara = tbInventar.Text;

            // Validacija pomoću regex-a
            bool isNazivJelaValid = Regex.IsMatch(nazivJela, @"^[a-žA-Ž\s-]+$");
            bool isCijenaJelaValid = Regex.IsMatch(cijenaJela, @"^\d+(\.\d{1,2})?$");
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

            if (!isNazivJelaValid)
            {
                errorMessage.AppendLine("Naziv jela može sadržavati samo slova i znak '-'.");
            }
            if (!isCijenaJelaValid)
            {
                errorMessage.AppendLine("Cijena jela mora biti u obliku kao npr:\n     12000.5 ili 12.99.");
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
                messageWindow.Top = 0; // Postavi Y koordinatu na vrh ekrana
                messageWindow.Left = (SystemParameters.PrimaryScreenWidth - messageWindow.Width) / 2; // Centriraj X koordinatu
                messageWindow.Show();
                return; // Prekini daljnje izvršavanje
            }
            // Kreiranje novog jela
            Jelo novoJelo = new Jelo
            {
                naziv = nazivJela,
                cijena = cijenaJela,
                nutrivne_informacije = nutritivneInformacije,
                alergeni = alergeni,
                Inventar_id_inventar = int.Parse(idInventara)
            };

            // Provjera slike
            if (isImageChanged)
            {
                BitmapImage bitmap = imgSlika.Source as BitmapImage;
                if (bitmap != null)
                {
                    novoJelo.slika = ImageToByte(bitmap);
                }
            }

            // Dodavanje novog jela
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
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
                            //obrisi recenzije vezane za jelo
                            List<Recenzija> vezaneRecenzije = await recenzijaServices.GetRecenzijeByIdAsync(jelo.id_jelo);

                            foreach (var jeloZaBrisanje in vezaneRecenzije)
                            {
                                recenzijaServices.RemoveRecenziju(jeloZaBrisanje);
                            }



                            //obrisi sve vezane stavke narudzbe i narudzbe, pa onda jelo
                            stavkeZaBrisanje = await stavka_NarudzbeServices.GetAllStavkeNarudzbeByJeloIdAsync(jelo.id_jelo);

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


                            jeloServices.RemoveJelo(jelo);
                            loadingText.Visibility = Visibility.Visible;
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
            tbInventarUredi.Text = globalnoJeloEdit.Inventar_id_inventar.ToString();

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
            // Provjera da li su sva polja popunjena
            if (string.IsNullOrEmpty(tbNazivJelaUredi.Text) || string.IsNullOrEmpty(tbCijenaUredi.Text) ||
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
            string nazivJela = tbNazivJelaUredi.Text.Trim();
            string cijenaJela = tbCijenaUredi.Text.Trim();
            string nutritivneInformacije = tbNutrivneInformacijeUredi.Text.Trim();
            string alergeni = tbAlergeniUredi.Text.Trim();
            string idInventara = tbInventarUredi.Text.Trim();

            // Validacija pomoću regex-a
            bool isNazivJelaValid = Regex.IsMatch(nazivJela, @"^[a-žA-Ž\s-]+$");
            bool isCijenaJelaValid = Regex.IsMatch(cijenaJela, @"^\d+(\.\d{1,2})?$");
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

            if (!isNazivJelaValid)
            {
                errorMessage.AppendLine("Naziv jela može sadržavati samo slova i znak '-'.");
            }
            if (!isCijenaJelaValid)
            {
                errorMessage.AppendLine("Cijena jela mora biti u obliku kao npr:\n     12000.5 ili 12.99.");
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

            globalnoJeloEdit.naziv = tbNazivJelaUredi.Text;
            globalnoJeloEdit.cijena = tbCijenaUredi.Text;
            globalnoJeloEdit.nutrivne_informacije = tbNutrivneInformacijeUredi.Text;
            globalnoJeloEdit.alergeni = tbAlergeniUredi.Text;
            globalnoJeloEdit.Inventar_id_inventar = int.Parse(tbInventarUredi.Text);

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
                //UcitajJela();
            }
        }
    }
}
