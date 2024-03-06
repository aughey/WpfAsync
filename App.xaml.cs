﻿using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfAsync
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Create and show the secondary window
            SecondaryWindow secondaryWindow = new SecondaryWindow();
            secondaryWindow.Show();
        }
    }

}
