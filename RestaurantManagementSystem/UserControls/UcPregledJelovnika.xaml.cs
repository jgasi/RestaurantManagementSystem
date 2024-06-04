using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcPregledJelovnika : UserControl
    {
        private JeloServices jeloServices = new JeloServices();
        public Jelo SelectedJelo { get; set; }
        public List<Jelo> SvaJela;
        private int currentPage = 0;
        private int itemsPerPage = 3;
        private bool isLoading = false;

        public ObservableCollection<Jelo> CurrentPageJela { get; set; }

        public UcPregledJelovnika()
        {
            InitializeComponent();
            CurrentPageJela = new ObservableCollection<Jelo>();
            DataContext = this;
            PrikaziStranicuFirstThree();
            UcitajSvaJelaAsync();
        }

        private async void PrikaziStranicuFirstThree()
        {
            var allItems = new List<Jelo>();

            var first = await jeloServices.GetFirstThreeJelaAsync(2);
            allItems.AddRange(first);

            var second = await jeloServices.GetFirstThreeJelaAsync(3);
            allItems.AddRange(second);

            var third = await jeloServices.GetFirstThreeJelaAsync(4);
            allItems.AddRange(third);

            foreach (var item in allItems)
            {
                CurrentPageJela.Add(item);
            }
            loadingText.Visibility = Visibility.Collapsed;
        }

        private async void UcitajSvaJelaAsync()
        {
            isLoading = true;

            try
            {
                SvaJela = await jeloServices.GetAllJelaAsync();
            }
            finally
            {
                isLoading = false;
                PrikaziStranicu(); // Poziv funkcije za prikaz stranice nakon što su sva jela učitana
            }
        }

        private void PrikaziStranicu()
        {
            if (isLoading)
            {
                loadingText.Visibility = Visibility.Visible;
                return;
            }

            List<Jelo> items = null;
            CurrentPageJela.Clear();

            if (SvaJela != null)
            {
                items = SvaJela.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();

                foreach (var item in items)
                {
                    CurrentPageJela.Add(item);
                }

                loadingText.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Prikaži "loading..." ako još uvijek nisu učitana sva jela
                loadingText.Visibility = Visibility.Visible;
                CurrentPageJela.Clear(); // Obriši trenutna jela
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = (currentPage + 1) * itemsPerPage < (SvaJela?.Count ?? 0);
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                PrikaziStranicu();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
            {
                currentPage++;
                PrikaziStranicu();
            }
            else
            {
                CurrentPageJela.Clear();
                loadingText.Visibility = Visibility.Visible;
                currentPage++;
            }
        }

        private void Jelo_Click(object sender, RoutedEventArgs e)
        {
            SelectedJelo = (sender as FrameworkElement)?.DataContext as Jelo;

            if (SelectedJelo != null)
            {
                UcDetaljiJela detaljiJela = new UcDetaljiJela(SelectedJelo);
                detaljiJela.DataContext = SelectedJelo;
                glavniGrid.Children.Clear();
                glavniGrid.Children.Add(detaljiJela);
            }
        }
    }
}
