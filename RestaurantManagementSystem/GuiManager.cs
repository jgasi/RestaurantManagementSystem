using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RestaurantManagementSystem
{
    public static class GuiManager
    {
        public static MainWindow mainWindow { get; set; }

        private static UserControl currentContent;
        private static UserControl previousContent;

        public static void OpenContent(UserControl userControl)
        {
            previousContent = mainWindow.contentPanel.Content as UserControl;
            mainWindow.contentPanel.Content = userControl;
            currentContent = mainWindow.contentPanel.Content as UserControl;
        }

        public static void CloseContent()
        {
            OpenContent(previousContent);
        }
    }
}
