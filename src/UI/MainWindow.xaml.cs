/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System.Windows;
using PsychoBuddy.UI;

namespace PsychoBuddy.UI
{
    public partial class MainWindow : Window
    {
        private DashboardController _controller;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new DashboardController();
            this.DataContext = _controller;
            _controller.InitializeFleet();
        }
    }
}
