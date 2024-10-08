﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using EntitiesLayer.Entities;
using FontAwesome.Sharp;
using RestaurantManagementSystem.UserControls;

namespace RestaurantManagementSystem
{
    public partial class MainWindow : Window
    {
        private Korisnik TrenutniKorisnik;


        public MainWindow()
        {
            InitializeComponent();
            UcProfil.ProfileImageChanged += OnProfileImageChanged;
        }

        public MainWindow(Korisnik korisnik) : this()
        {
            GuiManager.mainWindow = this;
            TrenutniKorisnik = korisnik;
            UcitajSlikuProfila();
        }

        private void UcitajSlikuProfila()
        {
            if (TrenutniKorisnik.slika != null)
            {
                BitmapImage bitmap = ByteToImage(TrenutniKorisnik.slika);
                profileImageBrush.ImageSource = bitmap;
            }
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

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void ProfilButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.User;
            pageTitle.Text = "Profil";
            var ucProfil = new UcProfil(TrenutniKorisnik);
            GuiManager.OpenContent(ucProfil);
        }

        private void Logout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GuiManager.Logout();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void LogoutTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            // Podcrtaj tekst kada se miš nalazi iznad teksta
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.TextDecorations = TextDecorations.Underline;
            }
        }

        private void LogoutTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            // Ukloni podcrtavanje teksta kada miš napusti tekst
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.TextDecorations = null;
            }
        }

        private void LogoutIconImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void PregledJelovnikaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.Utensils;
            pageTitle.Text = "Pregled jelovnika";
            var ucPregledJelovnika = new UcPregledJelovnika(TrenutniKorisnik);
            GuiManager.OpenContent(ucPregledJelovnika);
        }

        private void PregledPicaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.WineBottle;
            pageTitle.Text = "Pregled pića";
            var ucPregledPica = new UcPregledPica(TrenutniKorisnik);
            GuiManager.OpenContent(ucPregledPica);
        }

        private void OnProfileImageChanged(object sender, EventArgs e)
        {
            UcitajSlikuProfila();
        }

        private void KontaktInfoButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.Phone;
            pageTitle.Text = "Kontakt informacije";
            var ucKontaktInfo = new UcKontaktInformacije();
            GuiManager.OpenContent(ucKontaktInfo);
        }

        private void RezervacijaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.CalendarDays;
            pageTitle.Text = "Rezervacija";
            var ucRezervacija = new UcRezervacija(TrenutniKorisnik);
            GuiManager.OpenContent(ucRezervacija);
        }

        private void PovijestNarudzbiButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.ClockRotateLeft;
            pageTitle.Text = "Povijest narudžbi";
            var ucPovijestNarudzbi = new UcPovijestNarudzbi(TrenutniKorisnik);
            GuiManager.OpenContent(ucPovijestNarudzbi);
        }

        private void MojeRezervacijeButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.CalendarDays;
            pageTitle.Text = "Moje rezervacije";
            var ucMojeRezervacije = new UcMojeRezervacije(TrenutniKorisnik);
            GuiManager.OpenContent(ucMojeRezervacije);
        }
    }
}
