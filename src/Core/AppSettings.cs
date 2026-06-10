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
        public string PreferredSensesMode { get; set; } = "Power";
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
