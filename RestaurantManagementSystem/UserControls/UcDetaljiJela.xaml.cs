using System;
using System.Collections.Generic;
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
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    /// <summary>
    /// Interaction logic for UcDetaljiJela.xaml
    /// </summary>
    public partial class UcDetaljiJela : UserControl
    {
        public Jelo primljenoJelo;
        public UcDetaljiJela(Jelo selectedJelo)
        {
            InitializeComponent();
            DataContext = selectedJelo;
            primljenoJelo = selectedJelo;
            UcitajPodatke();
        }

        private void UcitajPodatke()
        {
            nazivTxtBl.Text = primljenoJelo.naziv;
            cijenaTxtBl.Text = primljenoJelo.cijena;
            nutrivne_informacijeTxtBl.Text = primljenoJelo.nutrivne_informacije;
            alergeniTxtBl.Text = primljenoJelo.alergeni;
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
