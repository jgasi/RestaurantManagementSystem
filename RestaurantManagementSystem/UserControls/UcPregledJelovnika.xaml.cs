using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcPregledJelovnika : UserControl
    {
        private JeloServices jeloServices = new JeloServices();
        private List<Jelo> Jela { get; set; }
        public Jelo SelectedJelo { get; set; }
        private int currentPage = 0;
        private int itemsPerPage = 8;

        public ObservableCollection<Jelo> CurrentPageJela { get; set; }

        public UcPregledJelovnika()
        {
            InitializeComponent();
            CurrentPageJela = new ObservableCollection<Jelo>();
            DataContext = this;
            UcitajJela();
            PrikaziStranicu();
        }

        private void UcitajJela()
        {
            Jela = jeloServices.GetAllJela();
        }

        private void PrikaziStranicu()
        {
            CurrentPageJela.Clear();
            var items = Jela.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();
            foreach (var item in items)
            {
                CurrentPageJela.Add(item);
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            PrevButton.IsEnabled = currentPage > 0;
            NextButton.IsEnabled = (currentPage + 1) * itemsPerPage < Jela.Count;
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
            if ((currentPage + 1) * itemsPerPage < Jela.Count)
            {
                currentPage++;
                PrikaziStranicu();
            }
        }

        // UcPregledJelovnika.xaml.cs

        private void Jelo_Click(object sender, RoutedEventArgs e)
        {
            SelectedJelo = (sender as FrameworkElement)?.DataContext as Jelo;

            if (SelectedJelo != null)
            {
                // Stvorite UserControl za prikaz detalja jela
                UcDetaljiJela detaljiJela = new UcDetaljiJela(SelectedJelo);

                // Postavite DataContext UserControl-a za prikaz detalja na odabrano jelo
                detaljiJela.DataContext = SelectedJelo;

                // Dodajte UserControl za prikaz detalja jela u isti Grid kao i UcPregledJelovnika
                glavniGrid.Children.Clear();
                glavniGrid.Children.Add(detaljiJela);
            }
        }


    }
}
