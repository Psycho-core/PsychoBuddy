/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

namespace PsychoBuddy.Core
{
    public class AppSettings
    {
        public string CustomWowExecutablePath { get; set; } = string.Empty;
        public string DefaultRole { get; set; } = "DPS";
        public string DefaultProfile { get; set; } = "Basic Rotation";
        public string TargetGameVersion { get; set; } = "BFA_8_3_7";
        public string PreferredSensesMode { get; set; } = "Power";
        public int SensesScanIntervalMs { get; set; } = 50;
        public int StealthPixelGridX { get; set; } = 0;
        public int StealthPixelGridY { get; set; } = 0;
        public int StealthPixelGridSize { get; set; } = 10;
        public bool EnableFlickerSuppression { get; set; } = true;
        public bool EnableBlackoutCheck { get; set; } = true;
        public bool UseWindowCapture { get; set; } = true;
        public string NavigationMode { get; set; } = "Follow";
        public int FollowDistance { get; set; } = 10;
        public int FormationSpacing { get; set; } = 5;
        public int WaypointRadius { get; set; } = 3;
        public bool AutoFollowLeader { get; set; } = true;
        public bool AvoidOverlap { get; set; } = true;
        public bool ShowNavigationPath { get; set; } = true;
        public string LastRouteName { get; set; } = "Default Route";
        public bool AutoScanOnStartup { get; set; } = true;
        public bool DebugMode { get; set; } = true;
        public string LogVerbosity { get; set; } = "Normal";
        public bool RememberWindowPlacement { get; set; } = true;
        public double LastWindowLeft { get; set; } = -1;
        public double LastWindowTop { get; set; } = -1;
        public double LastWindowWidth { get; set; } = 1700;
        public double LastWindowHeight { get; set; } = 925;
    }
}
