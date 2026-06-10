/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Windows;
using PsychoBuddy.UI;

namespace PsychoBuddy
{
    /// <summary>
    /// WPF application entry point. The generated App.g.cs Main method starts here.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                MainWindow mainWindow = new MainWindow();
                MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"PsychoBuddy failed to start.\n\n{ex.Message}",
                    "PsychoBuddy Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown(-1);
            }
        }
    }
}
