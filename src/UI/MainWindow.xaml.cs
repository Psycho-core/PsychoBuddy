/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PsychoBuddy.UI
{
    public partial class MainWindow : Window
    {
        private readonly DashboardController _controller;
        private bool _initialized;

        public MainWindow()
        {
            InitializeComponent();

            _controller = new DashboardController();
            DataContext = _controller;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_initialized) return;

            _initialized = true;
            _controller.InitializeFleet();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            try
            {
                if (e.ClickCount == 2)
                {
                    WindowState = WindowState == System.Windows.WindowState.Maximized
                        ? System.Windows.WindowState.Normal
                        : System.Windows.WindowState.Maximized;
                }
                else
                {
                    DragMove();
                }
            }
            catch (InvalidOperationException)
            {
                // DragMove can throw if the mouse is no longer down; safe to ignore.
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyAction("Window minimized.");
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogTextBox.ScrollToEnd();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyPlaceholder("Main menu");
        }

        private void ProfilesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyPlaceholder("Profiles panel");
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyPlaceholder("Navigation panel");
        }

        private void SensesNavButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyPlaceholder("Senses settings panel");
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.NotifyPlaceholder("Settings panel");
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.RefreshClientScan();
        }

        private void EmptyButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.LoadStandbyFleet();
        }

        private void TickButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.Tick();
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.AttachSelectedClient();
        }

        private void DetachButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.DetachSelectedFleetCard();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartSelectedFleetCard();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.PauseSelectedFleetCard();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StopSelectedFleetCard();
        }

        private void StartAllButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartAllAttached();
        }

        private void StopAllButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StopAllAttached();
        }

        private void SensesToggle_Click(object sender, RoutedEventArgs e)
        {
            _controller.ToggleSensesMode(SensesToggle.IsChecked == true);
        }
    }
}
