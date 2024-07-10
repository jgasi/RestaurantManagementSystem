using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontAwesome.Sharp;
using EntitiesLayer.Entities;
using RestaurantManagementSystem.UserControls;
using RestaurantManagementSystem.Autentikacija;

namespace RestaurantManagementSystem
{
    public partial class AdminWindow : Window
    {
        private Korisnik TrenutniKorisnik;

        public AdminWindow(Korisnik korisnik)
        {
            InitializeComponent();
            GuiManager.adminWindow = this;
            TrenutniKorisnik = korisnik;
            UcProfil.ProfileImageChanged += OnProfileImageChanged;
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
            TextBlock textBlock = sender as TextBlock;
            if (textBlock != null)
            {
                textBlock.TextDecorations = TextDecorations.Underline;
            }
        }

        private void LogoutTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
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

        private void UpravljanjeKorisnicimaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.Users;
            pageTitle.Text = "Upravljanje korisnicima";
            var ucUpravljanjeKorisnicima = new UcUpravljanjeKorisnicima();
            GuiManager.OpenContent(ucUpravljanjeKorisnicima);
        }

        private void UpravljanjeJelovnikomButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.Utensils;
            pageTitle.Text = "Upravljanje jelovnikom";
            var ucUpravljanjeJelovnikom = new UcUpravljanjeJelovnikom();
            GuiManager.OpenContent(ucUpravljanjeJelovnikom);
        }

        private void StatistikaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.ChartBar;
            pageTitle.Text = "Statistika";
            var ucStatistika = new UcStatistika();
            GuiManager.OpenContent(ucStatistika);
        }

        private void UpravljanjeZalihamaButton_Click(object sender, RoutedEventArgs e)
        {
            pageIcon.Icon = IconChar.Truck;
            pageTitle.Text = "Upravljanje zalihama";
            var ucUpravljanjeZalihama = new UcUpravljanjeZalihama();
            GuiManager.OpenContent(ucUpravljanjeZalihama);
        }
    }
}
