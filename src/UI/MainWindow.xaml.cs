/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

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
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_initialized) return;

            _initialized = true;
            _controller.ApplyWindowPlacement(this);
            SensesToggle.IsChecked = _controller.IsPowerModeSelected;
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
            _controller.OpenProfilesPanel();
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.OpenNavigationPanel();
        }

        private void SensesNavButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.OpenSensesPanel();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.OpenSettingsPanel();
        }


        private void UseSelectedProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.UseSelectedProfileFromProfilesPanel();
        }

        private void ReloadProfilesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ReloadProfiles();
        }

        private void CloseProfilesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.CloseProfilesPanel();
        }

        private void SaveNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.SaveNavigationPanel();
        }

        private void CancelNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.CancelNavigationPanel();
        }

        private void TestNavigationButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.TestNavigationConfiguration();
        }

        private void AddWaypointButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddNavigationWaypoint();
        }

        private void RemoveWaypointButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.RemoveNavigationWaypoint();
        }

        private void ClearWaypointsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ClearNavigationWaypoints();
        }

        private void SaveSensesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.SaveSensesPanel();
            SensesToggle.IsChecked = _controller.IsPowerModeSelected;
        }

        private void CancelSensesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.CancelSensesPanel();
            SensesToggle.IsChecked = _controller.IsPowerModeSelected;
        }

        private void TestSensesButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.TestSensesConfiguration();
        }

        private void BrowseWowPathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select World of Warcraft executable",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) == true)
            {
                _controller.CustomWowExecutablePath = dialog.FileName;
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.SaveSettingsPanel();
            SensesToggle.IsChecked = _controller.IsPowerModeSelected;
        }

        private void CancelSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.CancelSettingsPanel();
            SensesToggle.IsChecked = _controller.IsPowerModeSelected;
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

        private void AddMockClientButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.AddMockClient();
        }

        private void TestProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.TestSelectedProfileExecution();
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

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            _controller.CaptureWindowPlacement(this);
        }
    }
}
