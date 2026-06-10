/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System.Windows;

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

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.RefreshClientScan();
        }

        private void DemoButton_Click(object sender, RoutedEventArgs e)
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
