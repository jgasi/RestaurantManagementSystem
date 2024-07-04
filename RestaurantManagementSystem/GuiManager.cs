﻿using System;
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
        public static AdminWindow adminWindow { get; set; }

        private static UserControl currentContent;
        private static UserControl previousContent;

        public static void SetMainWindow(MainWindow window)
        {
            mainWindow = window;
        }

        public static void SetAdminWindow(AdminWindow window)
        {
            adminWindow = window;
        }

        public static void OpenContent(UserControl userControl)
        {
            if (mainWindow != null)
            {
                previousContent = mainWindow.contentPanel.Content as UserControl;
                mainWindow.contentPanel.Content = userControl;
                currentContent = mainWindow.contentPanel.Content as UserControl;
            }
            else if (adminWindow != null)
            {
                previousContent = adminWindow.contentPanel.Content as UserControl;
                adminWindow.contentPanel.Content = userControl;
                currentContent = adminWindow.contentPanel.Content as UserControl;
            }
        }

        public static void CloseContent()
        {
            if (mainWindow != null)
            {
                OpenContent(previousContent);
            }
            else if (adminWindow != null)
            {
                OpenContent(previousContent);
            }
        }
    }
}
