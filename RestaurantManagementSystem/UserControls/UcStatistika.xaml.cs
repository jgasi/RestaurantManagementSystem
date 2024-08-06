using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;

namespace RestaurantManagementSystem.UserControls
{
    public partial class UcStatistika : UserControl
    {
        private NarudzbaServices _narudzbaServices = new NarudzbaServices();
        private RecenzijaServices recenzijaServices = new RecenzijaServices();
        private Stavka_narudzbeServices stavka_NarudzbeServices = new Stavka_narudzbeServices();
        private JeloServices jeloServices = new JeloServices();
        private PiceServices piceServices = new PiceServices();
        private KorisnikServices korisnikServices = new KorisnikServices();

        public int najprodavanijeJeloId = 0;
        public int najprodavanijePiceId = 0;
        public int najgoreJeloId = 0;
        public int najgorePiceId = 0;


        public UcStatistika()
        {
            InitializeComponent();
            UcitajNajprodavanijeJelo();
            UcitajNajprodavanijePice();
            UcitajNajgoreJelo();
            UcitajNajgorePice();
            UcitajBrojKorisnika();
            UcitajBrojJela();
            UcitajBrojPica();
        }

        private async void GenerateStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Molimo odaberite početni i krajnji datum.");
                return;
            }

            var orders = await _narudzbaServices.GetNarudzbeByDateAsync(startDate, endDate);

            if (orders.Count == 0)
            {
                MessageBox.Show("Nema narudžbi u odabranom periodu.");
                return;
            }

            decimal totalRevenue = 0;
            int totalSoldUnits = 0;

            foreach (var order in orders)
            {
                string[] lines = order.racun.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.Contains("Ukupna cijena:"))
                    {
                        string totalPriceStr = line.Replace("Ukupna cijena:", "").Replace("EUR", "").Trim();
                        if (decimal.TryParse(totalPriceStr.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal totalPrice))
                        {
                            totalRevenue += totalPrice;
                        }
                    }
                    else
                    {
                        // Pretpostavljamo da svaka stavka sadrži jednu crticu '-'
                        if (line.Contains("-"))
                        {
                            totalSoldUnits++;
                        }
                    }
                }
            }

            var sveRecenzije = await recenzijaServices.GetAllRecenzijeAsync();

            int zbrojOcjena = 0;
            int kolikoOcjena = 0;

            foreach (var recenzija in sveRecenzije)
            {
                if (int.TryParse(recenzija.ocjena, out int trenutnaOcjena))
                {
                    kolikoOcjena++;
                    zbrojOcjena += trenutnaOcjena;
                }
            }
            decimal prosjekOcjena = kolikoOcjena > 0 ? (decimal)zbrojOcjena / kolikoOcjena : 0;

            // totalSoldUnits se računa ispravno, nema potrebe za oduzimanje 2
            tbUkupniPrihod.Text = $"{totalRevenue:C}";
            tbBrojProdanihJedinica.Text = totalSoldUnits.ToString();
            tbProsjecnaOcjena.Text = $"{prosjekOcjena:F2}";
        }


        private async void UcitajNajprodavanijeJelo()
        {
            try
            {
                var stavkeNarudzbe = await stavka_NarudzbeServices.GetAllStavkeNarudzbeAsync();

                // Filtriranje stavki koje imaju Jelo_id_jelo različit od null
                List<Stavka_narudzbe> jeloStavke = stavkeNarudzbe
                    .Where(s => s.Jelo_id_jelo != null)
                    .ToList();

                // Grupiranje po Jelo_id_jelo i brojanje ponavljanja
                var najcesciId = jeloStavke
                    .GroupBy(s => s.Jelo_id_jelo)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { JeloId = g.Key, BrojPonavljanja = g.Count() })
                    .FirstOrDefault();

                if (najcesciId != null)
                {
                    int jeloId = najcesciId.JeloId.GetValueOrDefault();
                    List<Jelo> najprodavanijeJeloList = await jeloServices.GetJeloByIdAsync(jeloId);

                    najprodavanijeJeloId = jeloId;

                    UcitajOcjeneJela();

                    Jelo pravoJelo = najprodavanijeJeloList.FirstOrDefault();

                    // Prikaz slike, naziva i cijene najprodavanijeg proizvoda
                    if (pravoJelo != null)
                    {
                        byte[] imageData = pravoJelo.slika; // Pretpostavka da imate byte[] polje za sliku u Jelo objektu
                        BitmapImage bitmapImage = ByteToImage(imageData);

                        // Postavljanje slike u XAML kontrolu
                        imgNajprodavanijeJelo.Source = bitmapImage; // imgNajprodavanijiProizvod je ime vaše Image XAML kontrolu

                        // Postavljanje naziva i cijene proizvoda
                        tbNajprodavanijeJeloNaziv.Text = pravoJelo.naziv;
                        tbNajprodavanijeJeloCijena.Text = $"{pravoJelo.cijena:C} €";
                    }
                    else
                    {
                        tbNajprodavanijeJeloNaziv.Text = "Nema podataka";
                        tbNajprodavanijeJeloCijena.Text = string.Empty;
                    }
                }
                else
                {
                    tbNajprodavanijeJeloNaziv.Text = "Nema podataka";
                    tbNajprodavanijeJeloCijena.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju najprodavanijeg proizvoda: {ex.Message}");
            }
        }

        private async void UcitajOcjeneJela()
        {
            try
            {
                List<Recenzija> recenzijeJela = await recenzijaServices.GetRecenzijeByIdAsync(najprodavanijeJeloId);

                if (recenzijeJela.Any())
                {
                    // Izračun prosječne ocjene
                    double prosjecnaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Average(r => int.Parse(r.ocjena));

                    tbNajprodavanijeJeloProsjecnaOcjena.Text = prosjecnaOcjena.ToString();

                    // Najveća ocjena
                    int najvecaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Max(r => int.Parse(r.ocjena));

                    // Najmanja ocjena
                    int najmanjaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Min(r => int.Parse(r.ocjena));

                    // Ažuriranje TextBlock-ova za najveću i najmanju ocjenu
                    tbNajprodavanijeJeloNajboljaOcjena.Text = najvecaOcjena.ToString();
                    tbNajprodavanijeJeloNajgoraOcjena.Text = najmanjaOcjena.ToString();
                }
                else
                {
                    // Ako nema recenzija
                    tbNajprodavanijeJeloProsjecnaOcjena.Text = "Nema recenzija";
                    tbNajprodavanijeJeloNajboljaOcjena.Text = "Nema recenzija";
                    tbNajprodavanijeJeloNajgoraOcjena.Text = "Nema recenzija";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju ocjena jela: {ex.Message}");
            }
        }


        private async void UcitajNajprodavanijePice()
        {
            try
            {
                var stavkeNarudzbe = await stavka_NarudzbeServices.GetAllStavkeNarudzbeAsync();

                // Filtriranje stavki koje imaju Pice_id_pice različit od null
                List<Stavka_narudzbe> piceStavke = stavkeNarudzbe
                    .Where(s => s.Pice_id_pice != null)
                    .ToList();

                // Grupiranje po Pice_id_pice i brojanje ponavljanja
                var najcesciId = piceStavke
                    .GroupBy(s => s.Pice_id_pice)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { PiceId = g.Key, BrojPonavljanja = g.Count() })
                    .FirstOrDefault();

                if (najcesciId != null)
                {
                    int piceId = najcesciId.PiceId.GetValueOrDefault();
                    List<Pice> najprodavanijePiceList = await piceServices.GetPiceByIdAsync(piceId);

                    najprodavanijePiceId = piceId;

                    UcitajOcjenePica();

                    Pice pravoPice = najprodavanijePiceList.FirstOrDefault();

                    // Prikaz slike, naziva i cijene najprodavanijeg pića
                    if (pravoPice != null)
                    {
                        byte[] imageData = pravoPice.slika; // Pretpostavka da imate byte[] polje za sliku u Pice objektu
                        BitmapImage bitmapImage = ByteToImage(imageData);

                        // Postavljanje slike u XAML kontrolu
                        imgNajprodavanijePice.Source = bitmapImage; // imgNajprodavanijePice je ime vaše Image XAML kontrolu

                        // Postavljanje naziva i cijene pića
                        tbNajprodavanijePiceNaziv.Text = pravoPice.naziv;
                        tbNajprodavanijePiceCijena.Text = $"{pravoPice.cijena:C} €";
                    }
                    else
                    {
                        tbNajprodavanijePiceNaziv.Text = "Nema podataka";
                        tbNajprodavanijePiceCijena.Text = string.Empty;
                    }
                }
                else
                {
                    tbNajprodavanijePiceNaziv.Text = "Nema podataka";
                    tbNajprodavanijePiceCijena.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju najprodavanijeg pića: {ex.Message}");
            }
        }

        private async void UcitajOcjenePica()
        {
            try
            {
                List<Recenzija> recenzijePica = await recenzijaServices.GetRecenzijePicaByIdAsync(najprodavanijePiceId);

                if (recenzijePica.Any())
                {
                    // Izračun prosječne ocjene
                    double prosjecnaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Average(r => int.Parse(r.ocjena));

                    tbNajprodavanijePiceProsjecnaOcjena.Text = prosjecnaOcjena.ToString();

                    // Najveća ocjena
                    int najvecaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Max(r => int.Parse(r.ocjena));

                    // Najmanja ocjena
                    int najmanjaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Min(r => int.Parse(r.ocjena));

                    // Ažuriranje TextBlock-ova za najveću i najmanju ocjenu
                    tbNajprodavanijePiceNajboljaOcjena.Text = najvecaOcjena.ToString();
                    tbNajprodavanijePiceNajgoraOcjena.Text = najmanjaOcjena.ToString();
                }
                else
                {
                    // Ako nema recenzija
                    tbNajprodavanijePiceProsjecnaOcjena.Text = "Nema recenzija";
                    tbNajprodavanijePiceNajboljaOcjena.Text = "Nema recenzija";
                    tbNajprodavanijePiceNajgoraOcjena.Text = "Nema recenzija";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju ocjena pića: {ex.Message}");
            }
        }

        private async void UcitajNajgoreJelo()
        {
            try
            {
                var sveRecenzije = await recenzijaServices.GetAllRecenzijeAsync();
                var recenzijeJela = sveRecenzije
                    .Where(r => r.Jelo_id_jelo != null)
                    .ToList();

                var najgoraOcjenaJelo = recenzijeJela
                    .GroupBy(r => r.Jelo_id_jelo)
                    .OrderBy(g => g.Average(r => int.Parse(r.ocjena)))
                    .Select(g => new { JeloId = g.Key, ProsjecnaOcjena = g.Average(r => int.Parse(r.ocjena)) })
                    .FirstOrDefault();

                if (najgoraOcjenaJelo != null)
                {
                    int jeloId = najgoraOcjenaJelo.JeloId.GetValueOrDefault();
                    List<Jelo> najgoreJeloList = await jeloServices.GetJeloByIdAsync(jeloId);

                    najgoreJeloId = jeloId;

                    UcitajOcjeneNajgoregJela();

                    Jelo pravoJelo = najgoreJeloList.FirstOrDefault();

                    if (pravoJelo != null)
                    {
                        byte[] imageData = pravoJelo.slika;
                        BitmapImage bitmapImage = ByteToImage(imageData);
                        imgNajgoreJelo.Source = bitmapImage;
                        tbNajgoreJeloNaziv.Text = pravoJelo.naziv;
                        tbNajgoreJeloCijena.Text = $"{pravoJelo.cijena:C} €";
                    }
                    else
                    {
                        tbNajgoreJeloNaziv.Text = "Nema podataka";
                        tbNajgoreJeloCijena.Text = string.Empty;
                    }
                }
                else
                {
                    tbNajgoreJeloNaziv.Text = "Nema podataka";
                    tbNajgoreJeloCijena.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju najgoreg jela: {ex.Message}");
            }
        }

        private async void UcitajOcjeneNajgoregJela()
        {
            try
            {
                List<Recenzija> recenzijeJela = await recenzijaServices.GetRecenzijeByIdAsync(najgoreJeloId);

                if (recenzijeJela.Any())
                {
                    double prosjecnaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Average(r => int.Parse(r.ocjena));

                    tbNajgoreJeloProsjecnaOcjena.Text = prosjecnaOcjena.ToString();

                    int najvecaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Max(r => int.Parse(r.ocjena));

                    int najmanjaOcjena = recenzijeJela
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Min(r => int.Parse(r.ocjena));

                    tbNajgoreJeloNajboljaOcjena.Text = najvecaOcjena.ToString();
                    tbNajgoreJeloNajgoraOcjena.Text = najmanjaOcjena.ToString();
                }
                else
                {
                    tbNajgoreJeloProsjecnaOcjena.Text = "Nema recenzija";
                    tbNajgoreJeloNajboljaOcjena.Text = "Nema recenzija";
                    tbNajgoreJeloNajgoraOcjena.Text = "Nema recenzija";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju ocjena jela: {ex.Message}");
            }
        }

        private async void UcitajNajgorePice()
        {
            try
            {
                var sveRecenzije = await recenzijaServices.GetAllRecenzijeAsync();
                var recenzijePica = sveRecenzije
                    .Where(r => r.Pice_id_pice != null)
                    .ToList();

                var najgoraOcjenaPice = recenzijePica
                    .GroupBy(r => r.Pice_id_pice)
                    .OrderBy(g => g.Average(r => int.Parse(r.ocjena)))
                    .Select(g => new { PiceId = g.Key, ProsjecnaOcjena = g.Average(r => int.Parse(r.ocjena)) })
                    .FirstOrDefault();

                if (najgoraOcjenaPice != null)
                {
                    int piceId = najgoraOcjenaPice.PiceId.GetValueOrDefault();
                    List<Pice> najgorePiceList = await piceServices.GetPiceByIdAsync(piceId);

                    najgorePiceId = piceId;

                    UcitajOcjeneNajgoregPica();

                    Pice pravoPice = najgorePiceList.FirstOrDefault();

                    if (pravoPice != null)
                    {
                        byte[] imageData = pravoPice.slika;
                        BitmapImage bitmapImage = ByteToImage(imageData);
                        imgNajgorePice.Source = bitmapImage;
                        tbNajgorePiceNaziv.Text = pravoPice.naziv;
                        tbNajgorePiceCijena.Text = $"{pravoPice.cijena:C} €";
                    }
                    else
                    {
                        tbNajgorePiceNaziv.Text = "Nema podataka";
                        tbNajgorePiceCijena.Text = string.Empty;
                    }
                }
                else
                {
                    tbNajgorePiceNaziv.Text = "Nema podataka";
                    tbNajgorePiceCijena.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju najgoreg pića: {ex.Message}");
            }
        }

        private async void UcitajOcjeneNajgoregPica()
        {
            try
            {
                List<Recenzija> recenzijePica = await recenzijaServices.GetRecenzijePicaByIdAsync(najgorePiceId);

                if (recenzijePica.Any())
                {
                    double prosjecnaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Average(r => int.Parse(r.ocjena));

                    tbNajgorePiceProsjecnaOcjena.Text = prosjecnaOcjena.ToString();

                    int najvecaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Max(r => int.Parse(r.ocjena));

                    int najmanjaOcjena = recenzijePica
                        .Where(r => int.TryParse(r.ocjena, out _))
                        .Min(r => int.Parse(r.ocjena));

                    tbNajgorePiceNajboljaOcjena.Text = najvecaOcjena.ToString();
                    tbNajgorePiceNajgoraOcjena.Text = najmanjaOcjena.ToString();
                }
                else
                {
                    tbNajgorePiceProsjecnaOcjena.Text = "Nema recenzija";
                    tbNajgorePiceNajboljaOcjena.Text = "Nema recenzija";
                    tbNajgorePiceNajgoraOcjena.Text = "Nema recenzija";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju ocjena pića: {ex.Message}");
            }
        }

        private async void UcitajBrojKorisnika()
        {
            try
            {
                var brojKorisnika = await korisnikServices.GetKorisnikCountAsync();
                tbBrojKorisnika.Text = brojKorisnika.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju broja korisnika: {ex.Message}");
            }
        }

        private async void UcitajBrojJela()
        {
            try
            {
                var brojJela = await jeloServices.GetJeloCountAsync();
                tbBrojJela.Text = brojJela.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju broja korisnika: {ex.Message}");
            }
        }

        private async void UcitajBrojPica()
        {
            try
            {
                var brojPica = await piceServices.GetPiceCountAsync();
                tbBrojPica.Text = brojPica.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju broja korisnika: {ex.Message}");
            }
        }



        private byte[] ImageToByte(BitmapImage bitmapImage)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder;
                if (bitmapImage.UriSource.AbsolutePath.EndsWith(".png"))
                {
                    encoder = new PngBitmapEncoder();
                }
                else
                {
                    encoder = new JpegBitmapEncoder();
                }

                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        private BitmapImage ByteToImage(byte[] imageData)
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
}
