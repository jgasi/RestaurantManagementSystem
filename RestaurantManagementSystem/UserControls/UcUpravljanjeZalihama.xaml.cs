using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcUpravljanjeZalihama : UserControl
    {
        private InventarServices inventarServices = new InventarServices();
        private JeloServices jeloServices = new JeloServices();
        private PiceServices piceServices = new PiceServices();
        private Inventar trenutniInventar;

        public UcUpravljanjeZalihama()
        {
            InitializeComponent();
            UcitajInventar();
        }

        private async void UcitajInventar()
        {
            try
            {
                List<Inventar> inventarList = await inventarServices.GetAllInventareAsync();
                dgInventar.ItemsSource = inventarList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju inventara: {ex.Message}");
            }
        }

        private void BtnUredi_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Inventar inventar)
            {
                trenutniInventar = inventar;

                tbKolicinaNaZalihi.Text = trenutniInventar.kolicina_na_zalihi;
                tbMinimalnaKolicinaNarudzbe.Text = trenutniInventar.minimalna_kolicina_narudzbe;
                dpDatumNabave.SelectedDate = trenutniInventar.datum_nabave;
                tbDostavljac.Text = trenutniInventar.dostavljac;
                tbCijena.Text = trenutniInventar.cijena;

                popupUredi.IsOpen = true;
            }
        }

        private void BtnDodajNoviInventar_Click(object sender, RoutedEventArgs e)
        {
            tbNovaKolicinaNaZalihi.Text = string.Empty;
            tbNovaMinimalnaKolicinaNarudzbe.Text = string.Empty;
            dpNoviDatumNabave.SelectedDate = null;
            tbNoviDostavljac.Text = string.Empty;
            tbNovaCijena.Text = string.Empty;

            popupDodajNovi.IsOpen = true;
        }

        private void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            // Deklaracija varijabli
            string kolicinaNaZalihi = tbKolicinaNaZalihi.Text.Trim();
            string minimalnaKolicinaNarudzbe = tbMinimalnaKolicinaNarudzbe.Text.Trim();
            string dostavljac = tbDostavljac.Text.Trim();
            string cijena = tbCijena.Text.Trim();
            DateTime? datumNabave = dpDatumNabave.SelectedDate;

            // Validacija pomoću regex-a
            bool isKolicinaNaZalihiValid = Regex.IsMatch(kolicinaNaZalihi, @"^\d+$");
            bool isMinimalnaKolicinaNarudzbeValid = Regex.IsMatch(minimalnaKolicinaNarudzbe, @"^\d+$");
            bool isDostavljacValid = Regex.IsMatch(dostavljac, @"^[a-žA-Ž0-9\-\.\s]+$");
            bool isCijenaValid = Regex.IsMatch(cijena, @"^\d+(\.\d{1,2})?$");

            // Provjera grešaka
            StringBuilder errorMessage = new StringBuilder();

            if (!isKolicinaNaZalihiValid)
            {
                errorMessage.AppendLine("Količina na zalihi mora biti cijeli broj.");
            }
            if (!isMinimalnaKolicinaNarudzbeValid)
            {
                errorMessage.AppendLine("Minimalna količina narudžbe mora biti cijeli broj.");
            }
            if (!isDostavljacValid)
            {
                errorMessage.AppendLine("Dostavljač može sadržavati slova, brojeve, znak '-' i '.'");
            }
            if (!isCijenaValid)
            {
                errorMessage.AppendLine("Cijena mora biti u obliku kao npr:\n     12000.99 ili 12.99.");
            }
            if (!datumNabave.HasValue) // Provjera da li je datum odabran
            {
                errorMessage.AppendLine("Morate odabrati datum nabave.");
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

            try
            {
                trenutniInventar.kolicina_na_zalihi = tbKolicinaNaZalihi.Text;
                trenutniInventar.minimalna_kolicina_narudzbe = tbMinimalnaKolicinaNarudzbe.Text;
                trenutniInventar.datum_nabave = dpDatumNabave.SelectedDate;
                trenutniInventar.dostavljac = tbDostavljac.Text;
                trenutniInventar.cijena = tbCijena.Text;

                inventarServices.UpdateInventar(trenutniInventar);

                popupUredi.IsOpen = false;
                MessageBox.Show("Inventar uspješno ažuriran.");
                UcitajInventar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri spremanju inventara: {ex.Message}");
            }
        }


        private void BtnSpremiNovi_Click(object sender, RoutedEventArgs e)
        {
            // Deklaracija varijabli
            string kolicinaNaZalihi = tbNovaKolicinaNaZalihi.Text.Trim();
            string minimalnaKolicinaNarudzbe = tbNovaMinimalnaKolicinaNarudzbe.Text.Trim();
            string dostavljac = tbNoviDostavljac.Text.Trim();
            string cijena = tbNovaCijena.Text.Trim();
            DateTime? datumNabave = dpNoviDatumNabave.SelectedDate;

            // Validacija pomoću regex-a
            bool isKolicinaNaZalihiValid = Regex.IsMatch(kolicinaNaZalihi, @"^\d+$");
            bool isMinimalnaKolicinaNarudzbeValid = Regex.IsMatch(minimalnaKolicinaNarudzbe, @"^\d+$");
            bool isDostavljacValid = Regex.IsMatch(dostavljac, @"^[a-žA-Ž0-9\-\.\s]+$");
            bool isCijenaValid = Regex.IsMatch(cijena, @"^\d+(\.\d{1,2})?$");

            // Provjera grešaka
            StringBuilder errorMessage = new StringBuilder();

            if (!isKolicinaNaZalihiValid)
            {
                errorMessage.AppendLine("Količina na zalihi mora biti cijeli broj.");
            }
            if (!isMinimalnaKolicinaNarudzbeValid)
            {
                errorMessage.AppendLine("Minimalna količina narudžbe mora biti cijeli broj.");
            }
            if (!isDostavljacValid)
            {
                errorMessage.AppendLine("Dostavljač može sadržavati slova, brojeve, znak '-' i '.'");
            }
            if (!isCijenaValid)
            {
                errorMessage.AppendLine("Cijena mora biti u obliku kao npr:\n     12000.99 ili 12.99.");
            }
            if (!datumNabave.HasValue) // Provjera da li je datum odabran
            {
                errorMessage.AppendLine("Morate odabrati datum nabave.");
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

            try
            {
                var noviInventar = new Inventar
                {
                    kolicina_na_zalihi = tbNovaKolicinaNaZalihi.Text,
                    minimalna_kolicina_narudzbe = tbNovaMinimalnaKolicinaNarudzbe.Text,
                    datum_nabave = dpNoviDatumNabave.SelectedDate,
                    dostavljac = tbNoviDostavljac.Text,
                    cijena = tbNovaCijena.Text
                };

                inventarServices.AddInventar(noviInventar);

                popupDodajNovi.IsOpen = false;
                MessageBox.Show("Novi inventar uspješno dodan.");
                UcitajInventar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju novog inventara: {ex.Message}");
            }
        }


        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            popupUredi.IsOpen = false;
        }

        private void BtnOdustaniNovi_Click(object sender, RoutedEventArgs e)
        {
            popupDodajNovi.IsOpen = false;
        }

        private async void ObrisiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Inventar inventar)
            {
                trenutniInventar = inventar;

                // Prikazivanje MessageBoxa za potvrdu
                var result = MessageBox.Show("Jeste li sigurni da želite obrisati inventar?", "Potvrda",
                                                            MessageBoxButton.YesNo,
                                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool jelIventarPrazan = await ProvjeriJelInventarZauzet(inventar.id_inventar);
                    if (jelIventarPrazan)
                    {
                        inventarServices.RemoveInventar(inventar);
                        UcitajInventar();
                    }
                    else
                    {
                        MessageBox.Show("Inventar nije prazan!\nNajprije obrišite jela ili pića iz inventara kako bi ga obrisali.");
                    }
                }
                // Ako je korisnik odabrao "No", ne radi ništa
            }
        }


        private async Task<bool> ProvjeriJelInventarZauzet(int id)
        {
            var jelaUInventaru = await jeloServices.GetJeloByIdInventaraAsync(id);
            var picaUInventaru = await piceServices.GetPiceByIdInventaraAsync(id);

            return !(jelaUInventaru?.Any() == true || picaUInventaru?.Any() == true);
        }


    }
}
