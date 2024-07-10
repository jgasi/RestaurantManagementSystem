using System;
using System.Collections.Generic;
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

        private void ObrisiButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Inventar inventar)
            {
                trenutniInventar = inventar;

                inventarServices.RemoveInventar(inventar);
                UcitajInventar();
            }
        }
    }
}
