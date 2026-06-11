/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using PsychoBuddy.Brain;
using PsychoBuddy.Core;
using PsychoBuddy.Muscles;
using PsychoBuddy.Senses;

namespace PsychoBuddy.UI
{
    /// <summary>
    /// Main Controller for the PsychoBuddy Dashboard.
    /// Handles the safe client workflow: Scan -> Configure -> Attach -> Start/Pause/Stop -> Detach.
    /// The Fleet Monitor is intentionally maintained as exactly four visible slots.
    /// </summary>
    public class DashboardController : INotifyPropertyChanged
    {
        private const int MaxFleetSlots = 4;

        private readonly Orchestrator _orchestrator;
        private readonly AttachmentManager _attachment;
        private readonly SettingsService _settingsService;
        private readonly ProfileService _profileService;
        private readonly Random _mockRandom = new Random();
        private AppSettings _settings;
        private int _mockClientCounter = 1;

        private string _statusMessage = "Command Center starting...";
        private string _sensesModeText = "Power Mode selected";
        private ClientBinding? _selectedAvailableClient;
        private FleetCardViewModel? _selectedFleetCard;
        private string _selectedRole = BotRole.DPS.ToString();
        private string _selectedProfile = "Basic Rotation";
        private bool _isSettingsPanelVisible;
        private string _customWowExecutablePath = string.Empty;
        private string _defaultRole = BotRole.DPS.ToString();
        private string _defaultProfile = "Basic Rotation";
        private string _targetGameVersion = "BFA_8_3_7";
        private string _preferredSensesMode = "Power";
        private int _sensesScanIntervalMs = 50;
        private int _stealthPixelGridX;
        private int _stealthPixelGridY;
        private int _stealthPixelGridSize = 10;
        private bool _enableFlickerSuppression = true;
        private bool _enableBlackoutCheck = true;
        private bool _useWindowCapture = true;
        private bool _isSensesPanelVisible;
        private bool _autoScanOnStartup = true;
        private bool _debugMode = true;
        private string _logVerbosity = "Normal";
        private bool _rememberWindowPlacement = true;
        private bool _isProfilesPanelVisible;
        private bool _isNavigationPanelVisible;
        private ProfileDefinition? _selectedProfileDefinition;
        private string _navigationMode = "Follow";
        private int _followDistance = 10;
        private int _formationSpacing = 5;
        private int _waypointRadius = 3;
        private bool _autoFollowLeader = true;
        private bool _avoidOverlap = true;
        private bool _showNavigationPath = true;
        private string _lastRouteName = "Default Route";
        private string? _selectedNavigationWaypoint;

        public DashboardController()
        {
            Fleet.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(FleetCountText));
                RefreshSelectionText();
            };

            AvailableClients.CollectionChanged += (_, _) => OnPropertyChanged(nameof(AvailableClientCountText));

            _settingsService = new SettingsService();
            _profileService = new ProfileService();
            _settings = _settingsService.Load();
            ReloadProfiles(silent: true);

            var input = new InputManager();
            _orchestrator = new Orchestrator(input);
            _attachment = new AttachmentManager();

            ApplySettingsToControllerState(_settings);
            _attachment.SetCustomProcessPath(_settings.CustomWowExecutablePath);
        }

        public ObservableCollection<ClientBinding> AvailableClients { get; } = new ObservableCollection<ClientBinding>();
        public ObservableCollection<FleetCardViewModel> Fleet { get; } = new ObservableCollection<FleetCardViewModel>();
        public ObservableCollection<string> LogEntries { get; } = new ObservableCollection<string>();
        public ObservableCollection<ProfileDefinition> Profiles { get; } = new ObservableCollection<ProfileDefinition>();
        public ObservableCollection<string> ProfileOptions { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> NavigationWaypoints { get; } = new ObservableCollection<string>();

        public IReadOnlyList<string> RoleOptions { get; } = Enum.GetNames<BotRole>();


        public IReadOnlyList<string> SensesModeOptions { get; } = new[]
        {
            "Power",
            "Stealth"
        };

        public IReadOnlyList<string> LogVerbosityOptions { get; } = new[]
        {
            "Quiet",
            "Normal",
            "Verbose",
            "Debug"
        };

        public IReadOnlyList<string> NavigationModeOptions { get; } = new[]
        {
            "Follow",
            "Waypoint",
            "Formation",
            "Manual"
        };

        public IReadOnlyList<string> GameVersionOptions => GameVersionCatalog.All;

        public ClientBinding? SelectedAvailableClient
        {
            get => _selectedAvailableClient;
            set
            {
                if (_selectedAvailableClient == value) return;
                _selectedAvailableClient = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedAvailableClientText));
            }
        }

        public FleetCardViewModel? SelectedFleetCard
        {
            get => _selectedFleetCard;
            set
            {
                if (_selectedFleetCard == value) return;
                _selectedFleetCard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedFleetCardText));
            }
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? BotRole.DPS.ToString() : value;
                if (_selectedRole == next) return;
                _selectedRole = next;
                OnPropertyChanged();
            }
        }

        public string SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? "Basic Rotation" : value;
                if (_selectedProfile == next) return;
                _selectedProfile = next;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string SensesModeText
        {
            get => _sensesModeText;
            private set
            {
                if (_sensesModeText == value) return;
                _sensesModeText = value;
                OnPropertyChanged();
            }
        }

        public bool IsProfilesPanelVisible
        {
            get => _isProfilesPanelVisible;
            private set
            {
                if (_isProfilesPanelVisible == value) return;
                _isProfilesPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public ProfileDefinition? SelectedProfileDefinition
        {
            get => _selectedProfileDefinition;
            set
            {
                if (_selectedProfileDefinition == value) return;
                _selectedProfileDefinition = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedProfileDetails));
                OnPropertyChanged(nameof(SelectedProfileDescription));
            }
        }

        public bool IsSettingsPanelVisible
        {
            get => _isSettingsPanelVisible;
            private set
            {
                if (_isSettingsPanelVisible == value) return;
                _isSettingsPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public string CustomWowExecutablePath
        {
            get => _customWowExecutablePath;
            set
            {
                string next = value ?? string.Empty;
                if (_customWowExecutablePath == next) return;
                _customWowExecutablePath = next;
                OnPropertyChanged();
            }
        }

        public string DefaultRole
        {
            get => _defaultRole;
            set
            {
                string next = NormalizeRole(value);
                if (_defaultRole == next) return;
                _defaultRole = next;
                OnPropertyChanged();
            }
        }

        public string DefaultProfile
        {
            get => _defaultProfile;
            set
            {
                string next = NormalizeProfile(value);
                if (_defaultProfile == next) return;
                _defaultProfile = next;
                OnPropertyChanged();
            }
        }

        public string TargetGameVersion
        {
            get => _targetGameVersion;
            set
            {
                string next = GameVersionCatalog.Normalize(value);
                if (_targetGameVersion == next) return;
                _targetGameVersion = next;
                OnPropertyChanged();
            }
        }

        public string PreferredSensesMode
        {
            get => _preferredSensesMode;
            set
            {
                string next = string.Equals(value, "Stealth", StringComparison.OrdinalIgnoreCase) ? "Stealth" : "Power";
                if (_preferredSensesMode == next) return;
                _preferredSensesMode = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPowerModeSelected));
            }
        }

        public bool AutoScanOnStartup
        {
            get => _autoScanOnStartup;
            set
            {
                if (_autoScanOnStartup == value) return;
                _autoScanOnStartup = value;
                OnPropertyChanged();
            }
        }

        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                if (_debugMode == value) return;
                _debugMode = value;
                OnPropertyChanged();
            }
        }

        public string LogVerbosity
        {
            get => _logVerbosity;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? "Normal" : value;
                if (_logVerbosity == next) return;
                _logVerbosity = next;
                OnPropertyChanged();
            }
        }

        public bool RememberWindowPlacement
        {
            get => _rememberWindowPlacement;
            set
            {
                if (_rememberWindowPlacement == value) return;
                _rememberWindowPlacement = value;
                OnPropertyChanged();
            }
        }

        public bool IsPowerModeSelected => !string.Equals(PreferredSensesMode, "Stealth", StringComparison.OrdinalIgnoreCase);

        public bool IsNavigationPanelVisible
        {
            get => _isNavigationPanelVisible;
            private set
            {
                if (_isNavigationPanelVisible == value) return;
                _isNavigationPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public string NavigationMode
        {
            get => _navigationMode;
            set
            {
                string next = NavigationModeOptions.Contains(value) ? value : "Follow";
                if (_navigationMode == next) return;
                _navigationMode = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public int FollowDistance
        {
            get => _followDistance;
            set
            {
                int next = Math.Clamp(value, 1, 100);
                if (_followDistance == next) return;
                _followDistance = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public int FormationSpacing
        {
            get => _formationSpacing;
            set
            {
                int next = Math.Clamp(value, 1, 50);
                if (_formationSpacing == next) return;
                _formationSpacing = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public int WaypointRadius
        {
            get => _waypointRadius;
            set
            {
                int next = Math.Clamp(value, 1, 50);
                if (_waypointRadius == next) return;
                _waypointRadius = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public bool AutoFollowLeader
        {
            get => _autoFollowLeader;
            set
            {
                if (_autoFollowLeader == value) return;
                _autoFollowLeader = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public bool AvoidOverlap
        {
            get => _avoidOverlap;
            set
            {
                if (_avoidOverlap == value) return;
                _avoidOverlap = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNavigationPath
        {
            get => _showNavigationPath;
            set
            {
                if (_showNavigationPath == value) return;
                _showNavigationPath = value;
                OnPropertyChanged();
            }
        }

        public string LastRouteName
        {
            get => _lastRouteName;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? "Default Route" : value;
                if (_lastRouteName == next) return;
                _lastRouteName = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NavigationSummary));
            }
        }

        public string? SelectedNavigationWaypoint
        {
            get => _selectedNavigationWaypoint;
            set
            {
                if (_selectedNavigationWaypoint == value) return;
                _selectedNavigationWaypoint = value;
                OnPropertyChanged();
            }
        }

        public string NavigationSummary => $"{NavigationMode} • Follow {FollowDistance} yd • Spacing {FormationSpacing} yd • Radius {WaypointRadius} yd";

        public bool IsSensesPanelVisible
        {
            get => _isSensesPanelVisible;
            private set
            {
                if (_isSensesPanelVisible == value) return;
                _isSensesPanelVisible = value;
                OnPropertyChanged();
            }
        }

        public int SensesScanIntervalMs
        {
            get => _sensesScanIntervalMs;
            set
            {
                int next = Math.Clamp(value, 10, 1000);
                if (_sensesScanIntervalMs == next) return;
                _sensesScanIntervalMs = next;
                OnPropertyChanged();
            }
        }

        public int StealthPixelGridX
        {
            get => _stealthPixelGridX;
            set
            {
                int next = Math.Max(0, value);
                if (_stealthPixelGridX == next) return;
                _stealthPixelGridX = next;
                OnPropertyChanged();
            }
        }

        public int StealthPixelGridY
        {
            get => _stealthPixelGridY;
            set
            {
                int next = Math.Max(0, value);
                if (_stealthPixelGridY == next) return;
                _stealthPixelGridY = next;
                OnPropertyChanged();
            }
        }

        public int StealthPixelGridSize
        {
            get => _stealthPixelGridSize;
            set
            {
                int next = Math.Clamp(value, 1, 64);
                if (_stealthPixelGridSize == next) return;
                _stealthPixelGridSize = next;
                OnPropertyChanged();
            }
        }

        public bool EnableFlickerSuppression
        {
            get => _enableFlickerSuppression;
            set
            {
                if (_enableFlickerSuppression == value) return;
                _enableFlickerSuppression = value;
                OnPropertyChanged();
            }
        }

        public bool EnableBlackoutCheck
        {
            get => _enableBlackoutCheck;
            set
            {
                if (_enableBlackoutCheck == value) return;
                _enableBlackoutCheck = value;
                OnPropertyChanged();
            }
        }

        public bool UseWindowCapture
        {
            get => _useWindowCapture;
            set
            {
                if (_useWindowCapture == value) return;
                _useWindowCapture = value;
                OnPropertyChanged();
            }
        }

        public string SensesSummary => $"{PreferredSensesMode} • {SensesScanIntervalMs}ms • Grid {StealthPixelGridSize}x{StealthPixelGridSize} @ {StealthPixelGridX},{StealthPixelGridY}";

        public string ProfileCountText => $"Profiles loaded: {Profiles.Count}";
        public string SelectedProfileDetails => SelectedProfileDefinition == null
            ? "No profile selected"
            : $"{SelectedProfileDefinition.DisplayName} — {SelectedProfileDefinition.MetadataSummary}";
        public string SelectedProfileDescription => SelectedProfileDefinition?.Description ?? "Select a profile to view details.";

        public string AvailableClientCountText => $"Available clients: {AvailableClients.Count}";
        public string FleetCountText => $"Attached fleet: {Fleet.Count(f => f.IsRealClient)} / {MaxFleetSlots}";
        public string SelectedAvailableClientText => SelectedAvailableClient == null
            ? "Selected available: none"
            : $"Selected available: {SelectedAvailableClient.CharacterName ?? "Unknown"} (PID {SelectedAvailableClient.Pid})";
        public string SelectedFleetCardText => SelectedFleetCard == null
            ? "Selected fleet: none"
            : $"Selected fleet: {SelectedFleetCard.CharacterName} [{SelectedFleetCard.Status}]";
        public string LogText => string.Join(Environment.NewLine, LogEntries);

        public event PropertyChangedEventHandler? PropertyChanged;

        public void InitializeFleet()
        {
            AddLog("PsychoBuddy Command Center initialized.");
            LoadStandbyFleet("Dashboard ready. Scan for clients to begin attachment workflow.");

            if (AutoScanOnStartup)
            {
                RefreshClientScan();
            }
            else
            {
                StatusMessage = "Auto-scan disabled. Click Scan when ready.";
                AddLog("Auto-scan on startup is disabled in Settings.");
            }
        }

        public void NotifyPlaceholder(string featureName)
        {
            NotifyAction($"{featureName} clicked. This panel/action is not implemented yet.");
        }

        public void NotifyAction(string message)
        {
            StatusMessage = message;
            AddLog(message);
        }

        public void OpenProfilesPanel()
        {
            IsSettingsPanelVisible = false;
            IsSensesPanelVisible = false;
            IsNavigationPanelVisible = false;
            ReloadProfiles();
            IsProfilesPanelVisible = true;
            StatusMessage = "Profiles panel opened.";
            AddLog("Profiles panel opened.");
        }

        public void CloseProfilesPanel()
        {
            IsProfilesPanelVisible = false;
            StatusMessage = "Profiles panel closed.";
            AddLog("Profiles panel closed.");
        }

        public void ReloadProfiles(bool silent = false)
        {
            string current = SelectedProfile;
            List<ProfileDefinition> loadedProfiles = _profileService.LoadProfiles();

            Profiles.Clear();
            ProfileOptions.Clear();

            foreach (ProfileDefinition profile in loadedProfiles)
            {
                Profiles.Add(profile);
                ProfileOptions.Add(profile.DisplayName);
            }

            if (ProfileOptions.Count == 0)
            {
                ProfileDefinition fallback = new ProfileDefinition();
                Profiles.Add(fallback);
                ProfileOptions.Add(fallback.DisplayName);
            }

            SelectedProfileDefinition = Profiles.FirstOrDefault(profile => profile.DisplayName == current) ?? Profiles.FirstOrDefault();
            if (SelectedProfileDefinition != null)
            {
                SelectedProfile = SelectedProfileDefinition.DisplayName;
            }

            OnPropertyChanged(nameof(ProfileCountText));

            if (!silent)
            {
                StatusMessage = $"Reloaded {Profiles.Count} profile(s).";
                AddLog(StatusMessage);
            }
        }

        public void UseSelectedProfileFromProfilesPanel()
        {
            if (SelectedProfileDefinition == null)
            {
                StatusMessage = "No profile selected.";
                AddLog("Profile selection skipped: no profile selected.");
                return;
            }

            SelectedProfile = SelectedProfileDefinition.DisplayName;
            DefaultProfile = SelectedProfileDefinition.DisplayName;
            IsProfilesPanelVisible = false;
            StatusMessage = $"Selected profile: {SelectedProfileDefinition.DisplayName}.";
            AddLog(StatusMessage);
        }

        public void OpenNavigationPanel()
        {
            IsProfilesPanelVisible = false;
            IsSettingsPanelVisible = false;
            IsSensesPanelVisible = false;
            ApplySettingsToControllerState(_settings);
            EnsureNavigationPlaceholders();
            IsNavigationPanelVisible = true;
            StatusMessage = "Navigation panel opened.";
            AddLog("Navigation panel opened.");
        }

        public void CancelNavigationPanel()
        {
            ApplySettingsToControllerState(_settings);
            IsNavigationPanelVisible = false;
            StatusMessage = "Navigation changes cancelled.";
            AddLog("Navigation panel closed without saving.");
        }

        public void SaveNavigationPanel()
        {
            _settings.NavigationMode = NavigationMode;
            _settings.FollowDistance = FollowDistance;
            _settings.FormationSpacing = FormationSpacing;
            _settings.WaypointRadius = WaypointRadius;
            _settings.AutoFollowLeader = AutoFollowLeader;
            _settings.AvoidOverlap = AvoidOverlap;
            _settings.ShowNavigationPath = ShowNavigationPath;
            _settings.LastRouteName = LastRouteName;
            _settingsService.Save(_settings);
            IsNavigationPanelVisible = false;
            StatusMessage = $"Navigation settings saved. {NavigationSummary}.";
            AddLog(StatusMessage);
        }

        public void TestNavigationConfiguration()
        {
            EnsureNavigationPlaceholders();
            StatusMessage = $"Navigation test: {NavigationSummary}. Pathfinding backend is still scaffolded.";
            AddLog(StatusMessage);
        }

        public void AddNavigationWaypoint()
        {
            int next = NavigationWaypoints.Count + 1;
            string waypoint = $"Waypoint {next}: awaiting live position";
            NavigationWaypoints.Add(waypoint);
            SelectedNavigationWaypoint = waypoint;
            StatusMessage = $"Added placeholder {waypoint}.";
            AddLog(StatusMessage);
        }

        public void RemoveNavigationWaypoint()
        {
            if (SelectedNavigationWaypoint == null)
            {
                StatusMessage = "No waypoint selected.";
                AddLog("Remove waypoint skipped: no waypoint selected.");
                return;
            }

            string removed = SelectedNavigationWaypoint;
            NavigationWaypoints.Remove(removed);
            SelectedNavigationWaypoint = NavigationWaypoints.FirstOrDefault();
            StatusMessage = $"Removed {removed}.";
            AddLog(StatusMessage);
        }

        public void ClearNavigationWaypoints()
        {
            NavigationWaypoints.Clear();
            SelectedNavigationWaypoint = null;
            StatusMessage = "Navigation waypoint list cleared.";
            AddLog(StatusMessage);
        }

        public void OpenSensesPanel()
        {
            IsProfilesPanelVisible = false;
            IsSettingsPanelVisible = false;
            IsNavigationPanelVisible = false;
            ApplySettingsToControllerState(_settings);
            IsSensesPanelVisible = true;
            StatusMessage = "Senses settings panel opened.";
            AddLog("Senses settings panel opened.");
        }

        public void CancelSensesPanel()
        {
            ApplySettingsToControllerState(_settings);
            IsSensesPanelVisible = false;
            StatusMessage = "Senses settings changes cancelled.";
            AddLog("Senses settings panel closed without saving.");
        }

        public void SaveSensesPanel()
        {
            _settings.PreferredSensesMode = PreferredSensesMode;
            _settings.SensesScanIntervalMs = SensesScanIntervalMs;
            _settings.StealthPixelGridX = StealthPixelGridX;
            _settings.StealthPixelGridY = StealthPixelGridY;
            _settings.StealthPixelGridSize = StealthPixelGridSize;
            _settings.EnableFlickerSuppression = EnableFlickerSuppression;
            _settings.EnableBlackoutCheck = EnableBlackoutCheck;
            _settings.UseWindowCapture = UseWindowCapture;

            _settingsService.Save(_settings);
            ToggleSensesMode(IsPowerModeSelected);
            IsSensesPanelVisible = false;
            StatusMessage = $"Senses settings saved. {SensesSummary}.";
            AddLog(StatusMessage);
        }

        public void TestSensesConfiguration()
        {
            StatusMessage = $"Senses test: {SensesSummary}. Backend capture is still scaffolded.";
            AddLog(StatusMessage);
        }

        public void OpenSettingsPanel()
        {
            IsProfilesPanelVisible = false;
            IsSensesPanelVisible = false;
            IsNavigationPanelVisible = false;
            ApplySettingsToControllerState(_settings);
            IsSettingsPanelVisible = true;
            StatusMessage = "Settings panel opened.";
            AddLog("Settings panel opened.");
        }

        public void CancelSettingsPanel()
        {
            ApplySettingsToControllerState(_settings);
            IsSettingsPanelVisible = false;
            StatusMessage = "Settings changes cancelled.";
            AddLog("Settings panel closed without saving.");
        }

        public void SaveSettingsPanel()
        {
            _settings.CustomWowExecutablePath = CustomWowExecutablePath.Trim();
            _settings.DefaultRole = NormalizeRole(DefaultRole);
            _settings.DefaultProfile = NormalizeProfile(DefaultProfile);
            _settings.TargetGameVersion = GameVersionCatalog.Normalize(TargetGameVersion);
            _settings.PreferredSensesMode = PreferredSensesMode;
            _settings.SensesScanIntervalMs = SensesScanIntervalMs;
            _settings.StealthPixelGridX = StealthPixelGridX;
            _settings.StealthPixelGridY = StealthPixelGridY;
            _settings.StealthPixelGridSize = StealthPixelGridSize;
            _settings.EnableFlickerSuppression = EnableFlickerSuppression;
            _settings.EnableBlackoutCheck = EnableBlackoutCheck;
            _settings.UseWindowCapture = UseWindowCapture;
            _settings.NavigationMode = NavigationMode;
            _settings.FollowDistance = FollowDistance;
            _settings.FormationSpacing = FormationSpacing;
            _settings.WaypointRadius = WaypointRadius;
            _settings.AutoFollowLeader = AutoFollowLeader;
            _settings.AvoidOverlap = AvoidOverlap;
            _settings.ShowNavigationPath = ShowNavigationPath;
            _settings.LastRouteName = LastRouteName;
            _settings.AutoScanOnStartup = AutoScanOnStartup;
            _settings.DebugMode = DebugMode;
            _settings.LogVerbosity = LogVerbosity;
            _settings.RememberWindowPlacement = RememberWindowPlacement;

            _settingsService.Save(_settings);
            _attachment.SetCustomProcessPath(_settings.CustomWowExecutablePath);

            SelectedRole = _settings.DefaultRole;
            SelectedProfile = _settings.DefaultProfile;
            ToggleSensesMode(IsPowerModeSelected);

            IsSettingsPanelVisible = false;
            StatusMessage = $"Settings saved to {_settingsService.SettingsPath}.";
            AddLog(StatusMessage);
        }

        public void ApplyWindowPlacement(Window window)
        {
            if (!_settings.RememberWindowPlacement) return;
            if (!IsUsableNumber(_settings.LastWindowWidth) || !IsUsableNumber(_settings.LastWindowHeight)) return;
            if (_settings.LastWindowWidth < 800 || _settings.LastWindowHeight < 600) return;

            window.Width = _settings.LastWindowWidth;
            window.Height = _settings.LastWindowHeight;

            if (IsUsableNumber(_settings.LastWindowLeft) && IsUsableNumber(_settings.LastWindowTop) &&
                _settings.LastWindowLeft >= 0 && _settings.LastWindowTop >= 0)
            {
                window.Left = _settings.LastWindowLeft;
                window.Top = _settings.LastWindowTop;
            }
        }

        public void CaptureWindowPlacement(Window window)
        {
            if (!RememberWindowPlacement) return;

            Rect restoreBounds = window.RestoreBounds;
            double width = IsUsableNumber(restoreBounds.Width) && restoreBounds.Width > 0 ? restoreBounds.Width : window.Width;
            double height = IsUsableNumber(restoreBounds.Height) && restoreBounds.Height > 0 ? restoreBounds.Height : window.Height;
            double left = IsUsableNumber(restoreBounds.Left) && restoreBounds.Left >= 0 ? restoreBounds.Left : window.Left;
            double top = IsUsableNumber(restoreBounds.Top) && restoreBounds.Top >= 0 ? restoreBounds.Top : window.Top;

            _settings.RememberWindowPlacement = RememberWindowPlacement;
            _settings.LastWindowWidth = IsUsableNumber(width) && width > 0 ? width : 1700;
            _settings.LastWindowHeight = IsUsableNumber(height) && height > 0 ? height : 925;
            _settings.LastWindowLeft = IsUsableNumber(left) && left >= 0 ? left : -1;
            _settings.LastWindowTop = IsUsableNumber(top) && top >= 0 ? top : -1;

            _settingsService.Save(_settings);
        }

        public void RefreshSettingsDefaultsFromSelection()
        {
            DefaultRole = SelectedRole;
            DefaultProfile = SelectedProfile;
        }

        public void RefreshClientScan()
        {
            EnsureFleetSlots();
            AddLog("Scanning for World of Warcraft clients...");
            AvailableClients.Clear();

            try
            {
                var alreadyAttachedPids = Fleet
                    .Where(card => card.Binding != null && card.Binding.Pid > 0)
                    .Select(card => card.Binding!.Pid)
                    .ToHashSet();

                var clients = _attachment.ScanForClients()
                    .Where(client => client.Pid > 0 && client.WindowHandle != IntPtr.Zero)
                    .Where(client => !alreadyAttachedPids.Contains(client.Pid))
                    .OrderBy(client => client.CharacterName)
                    .ToList();

                foreach (var client in clients)
                {
                    client.Status = BotStatus.Disconnected;
                    AvailableClients.Add(client);
                }

                SelectedAvailableClient = AvailableClients.FirstOrDefault();

                if (AvailableClients.Count == 0)
                {
                    StatusMessage = "No unattached clients found. Open WoW, then press Scan Clients.";
                    AddLog("No unattached clients found.");
                    return;
                }

                StatusMessage = $"Found {AvailableClients.Count} unattached client(s). Choose role/profile, then Attach.";
                AddLog($"Scan complete: {AvailableClients.Count} unattached client(s) ready for attachment.");
            }
            catch (Exception ex)
            {
                StatusMessage = "Client scan failed. Dashboard remains available.";
                AddLog($"Client scan failed: {ex.Message}");
            }
        }

        public void AttachSelectedClient()
        {
            EnsureFleetSlots();

            if (SelectedAvailableClient == null)
            {
                StatusMessage = "No available client selected.";
                AddLog("Attach skipped: select a client from Available Clients first.");
                return;
            }

            if (Fleet.Any(card => card.Binding?.Pid == SelectedAvailableClient.Pid))
            {
                StatusMessage = "Selected client is already attached.";
                AddLog($"Attach skipped: PID {SelectedAvailableClient.Pid} is already in the fleet.");
                return;
            }

            int slotIndex = FindFirstNonRealSlotIndex();
            if (slotIndex < 0)
            {
                StatusMessage = "Fleet is full. Detach a client before attaching another.";
                AddLog("Attach skipped: all four fleet slots are occupied by real clients.");
                return;
            }

            BotRole role = ParseSelectedRole();
            var binding = new ClientBinding
            {
                Pid = SelectedAvailableClient.Pid,
                WindowHandle = SelectedAvailableClient.WindowHandle,
                CharacterName = SelectedAvailableClient.CharacterName,
                AssignedProfile = SelectedProfile,
                TargetGameVersion = TargetGameVersion,
                Status = BotStatus.Attached
            };

            var card = new FleetCardViewModel
            {
                Binding = binding,
                Role = role.ToString(),
                AssignedProfile = SelectedProfile,
                Status = "Attached / Idle"
            };

            card.Update(new UnitData
            {
                HealthCurrent = 100,
                HealthMax = 100,
                ManaCurrent = 100,
                ManaMax = 100,
                Level = 0
            });

            ReplaceFleetSlot(slotIndex, card);
            SelectedFleetCard = card;

            AddLog($"Attached {binding.CharacterName ?? "Unknown"} as {role} using profile '{SelectedProfile}' in fleet slot {slotIndex + 1}.");
            StatusMessage = $"Attached {binding.CharacterName ?? "Unknown"}. Press Start Selected when ready.";

            AvailableClients.Remove(SelectedAvailableClient);
            SelectedAvailableClient = AvailableClients.FirstOrDefault();
        }

        public void DetachSelectedFleetCard()
        {
            EnsureFleetSlots();
            var card = SelectedFleetCard;

            if (card == null)
            {
                StatusMessage = "No fleet card selected.";
                AddLog("Detach skipped: select an attached fleet card first.");
                return;
            }

            if (!card.IsRealClient)
            {
                StatusMessage = "Selected fleet slot is not attached to a real client.";
                AddLog("Detach skipped: selected slot is empty/offline, not a real attached client.");
                return;
            }

            int slotIndex = Fleet.IndexOf(card);
            int pid = card.Binding!.Pid;
            string characterName = card.CharacterName;

            _orchestrator.UnregisterBot(pid);
            ReplaceFleetSlot(slotIndex, CreateEmptySlot(slotIndex));
            SelectedFleetCard = Fleet[slotIndex];

            AddLog($"Detached {characterName} (PID {pid}) from fleet slot {slotIndex + 1}.");
            StatusMessage = $"Detached {characterName}. Fleet slot {slotIndex + 1} is now empty.";
        }

        public void StartSelectedFleetCard()
        {
            if (SelectedFleetCard == null)
            {
                StatusMessage = "No fleet card selected.";
                AddLog("Start skipped: select an attached fleet card first.");
                return;
            }

            StartFleetCard(SelectedFleetCard);
        }

        public void PauseSelectedFleetCard()
        {
            var card = SelectedFleetCard;
            if (card?.Binding == null || !card.IsRealClient)
            {
                StatusMessage = "No real attached client selected.";
                AddLog("Pause skipped: select a real attached client first.");
                return;
            }

            card.SetStatus(BotStatus.Paused, "Paused");
            AddLog($"Paused {card.CharacterName}.");
            StatusMessage = $"Paused {card.CharacterName}.";
            RefreshSelectionText();
        }

        public void StopSelectedFleetCard()
        {
            if (SelectedFleetCard == null)
            {
                StatusMessage = "No fleet card selected.";
                AddLog("Stop skipped: select an attached fleet card first.");
                return;
            }

            StopFleetCard(SelectedFleetCard);
        }

        public void StartAllAttached()
        {
            EnsureFleetSlots();
            int started = 0;
            foreach (var card in Fleet.Where(f => f.IsRealClient).ToList())
            {
                if (StartFleetCard(card, quiet: true)) started++;
            }

            StatusMessage = started > 0 ? $"Started {started} attached client(s)." : "No attached clients were started.";
            AddLog(StatusMessage);
            RefreshSelectionText();
        }

        public void StopAllAttached()
        {
            EnsureFleetSlots();
            int stopped = 0;
            foreach (var card in Fleet.Where(f => f.IsRealClient).ToList())
            {
                if (StopFleetCard(card, quiet: true)) stopped++;
            }

            StatusMessage = stopped > 0 ? $"Stopped {stopped} attached client(s)." : "No attached clients were stopped.";
            AddLog(StatusMessage);
            RefreshSelectionText();
        }

        public void TestSelectedProfileExecution()
        {
            RotationProfile profile = _profileService.BuildRotationProfile(SelectedProfile);
            UnitData simulatedState = new UnitData
            {
                HealthCurrent = 50,
                HealthMax = 100,
                ManaCurrent = 90,
                ManaMax = 100,
                Level = 80
            };

            Ability? next = profile.GetNextAbility(simulatedState);
            string abilityName = next?.Name ?? "none";
            StatusMessage = $"Profile test: {profile.ProfileName} selected '{abilityName}' from {profile.Rules.Count} rule(s).";
            AddLog(StatusMessage);
        }

        public void AddMockClient()
        {
            EnsureFleetSlots();
            int slotIndex = FindFirstNonRealSlotIndex();
            if (slotIndex < 0)
            {
                StatusMessage = "Fleet is full. Detach a client before adding a mock client.";
                AddLog("Mock client skipped: all four fleet slots are occupied.");
                return;
            }

            string role = NormalizeRole(SelectedRole);
            string profile = NormalizeProfile(SelectedProfile);
            int mockId = 900000 + _mockClientCounter++;
            var binding = new ClientBinding
            {
                Pid = mockId,
                WindowHandle = IntPtr.Zero,
                CharacterName = $"Mock {role} {slotIndex + 1}",
                AssignedProfile = profile,
                TargetGameVersion = TargetGameVersion,
                IsMockClient = true,
                Status = BotStatus.Attached
            };

            var card = new FleetCardViewModel
            {
                Binding = binding,
                Role = role,
                AssignedProfile = profile,
                Level = 80,
                Status = "Mock / Idle"
            };

            card.Update(new UnitData
            {
                HealthCurrent = 100,
                HealthMax = 100,
                ManaCurrent = 100,
                ManaMax = 100,
                Level = 80
            });

            ReplaceFleetSlot(slotIndex, card);
            SelectedFleetCard = card;
            StatusMessage = $"Added mock client in fleet slot {slotIndex + 1}.";
            AddLog($"Added mock client '{binding.CharacterName}' with profile '{profile}' for {TargetGameVersion}.");
        }

        public void LoadStandbyFleet(string statusMessage = "Empty/offline fleet slots loaded.")
        {
            if (Fleet.Any(f => f.IsRealClient))
            {
                StatusMessage = "Empty slots cannot be reset while real clients are attached. Detach clients first.";
                AddLog(StatusMessage);
                return;
            }

            Fleet.Clear();
            for (int i = 0; i < MaxFleetSlots; i++)
            {
                Fleet.Add(CreateEmptySlot(i));
            }

            SelectedFleetCard = Fleet.FirstOrDefault();
            StatusMessage = statusMessage;
            AddLog("Empty fleet slots loaded in fixed 2x2 layout.");
        }

        public void Tick()
        {
            EnsureFleetSlots();
            var runningCards = Fleet
                .Where(f => f.IsRealClient && f.Binding?.Status == BotStatus.Running)
                .ToList();

            if (runningCards.Count == 0)
            {
                AddLog("Fleet tick skipped: no running clients.");
                StatusMessage = "Fleet tick skipped: no running clients.";
                return;
            }

            try
            {
                var realRunningCards = runningCards
                    .Where(card => card.Binding?.IsMockClient != true)
                    .ToList();
                var mockRunningCards = runningCards
                    .Where(card => card.Binding?.IsMockClient == true)
                    .ToList();

                if (realRunningCards.Count > 0)
                {
                    _orchestrator.UpdateFleet();

                    foreach (var card in realRunningCards)
                    {
                        UnitData? data = _orchestrator.GetLastUnitData(card.Binding!.Pid);
                        if (data != null) card.Update(data);
                    }
                }

                foreach (var card in mockRunningCards)
                {
                    SimulateMockClientTick(card);
                }

                StatusMessage = $"Fleet tick processed: {runningCards.Count} running client(s) ({mockRunningCards.Count} mock).";
                AddLog(StatusMessage);
            }
            catch (Exception ex)
            {
                StatusMessage = "Fleet tick failed. See technical log.";
                AddLog($"Fleet tick failed: {ex.Message}");
            }
        }

        public void ToggleSensesMode(bool powerMode)
        {
            PreferredSensesMode = powerMode ? "Power" : "Stealth";
            SensesModeText = powerMode ? "Power Mode selected" : "Stealth Mode selected";
            OnPropertyChanged(nameof(SensesSummary));
            OnPropertyChanged(nameof(NavigationSummary));
            AddLog($"Senses mode changed: {SensesModeText}.");
        }

        private bool StartFleetCard(FleetCardViewModel card, bool quiet = false)
        {
            ClientBinding? binding = card.Binding;
            if (binding == null || !card.IsRealClient)
            {
                if (!quiet)
                {
                    StatusMessage = "Selected fleet slot is not attached to a real client.";
                    AddLog("Start skipped: selected slot is empty/offline, not a real attached client.");
                }
                return false;
            }

            if (binding.IsMockClient)
            {
                card.SetStatus(BotStatus.Running, "Running");
                RefreshSelectionText();
                if (!quiet)
                {
                    StatusMessage = $"Started mock client {card.CharacterName}.";
                    AddLog($"Started mock client {card.CharacterName} with profile '{card.AssignedProfile}'.");
                }
                return true;
            }

            try
            {
                BotRole role = ParseRole(card.Role);
                RotationProfile profile = _profileService.BuildRotationProfile(card.AssignedProfile);

                if (!_orchestrator.IsRegistered(binding.Pid))
                {
                    _orchestrator.RegisterBot(binding, profile, role);
                }

                card.SetStatus(BotStatus.Running, "Running");
                RefreshSelectionText();
                if (!quiet)
                {
                    AddLog($"Started {card.CharacterName} with profile '{card.AssignedProfile}'.");
                    StatusMessage = $"Started {card.CharacterName}.";
                }
                return true;
            }
            catch (Exception ex)
            {
                card.SetStatus(BotStatus.Attached, "Start failed");
                RefreshSelectionText();
                AddLog($"Failed to start {card.CharacterName}: {ex.Message}");
                StatusMessage = $"Failed to start {card.CharacterName}. See technical log.";
                return false;
            }
        }

        private bool StopFleetCard(FleetCardViewModel card, bool quiet = false)
        {
            ClientBinding? binding = card.Binding;
            if (binding == null || !card.IsRealClient)
            {
                if (!quiet)
                {
                    StatusMessage = "Selected fleet slot is not attached to a real client.";
                    AddLog("Stop skipped: selected slot is empty/offline, not a registered client.");
                }
                return false;
            }

            if (!binding.IsMockClient)
            {
                _orchestrator.UnregisterBot(binding.Pid);
            }
            card.SetStatus(BotStatus.Stopped, "Stopped");
            RefreshSelectionText();

            if (!quiet)
            {
                AddLog($"Stopped {card.CharacterName}.");
                StatusMessage = $"Stopped {card.CharacterName}.";
            }
            return true;
        }

        private void EnsureFleetSlots()
        {
            while (Fleet.Count < MaxFleetSlots)
            {
                Fleet.Add(CreateEmptySlot(Fleet.Count));
            }

            while (Fleet.Count > MaxFleetSlots)
            {
                Fleet.RemoveAt(Fleet.Count - 1);
            }

            if (SelectedFleetCard == null && Fleet.Count > 0)
            {
                SelectedFleetCard = Fleet[0];
            }
        }

        private FleetCardViewModel CreateEmptySlot(int slotIndex)
        {
            var card = new FleetCardViewModel
            {
                Binding = new ClientBinding
                {
                    CharacterName = "Awaiting Client",
                    Status = BotStatus.Disconnected,
                    WindowHandle = IntPtr.Zero,
                    Pid = 0,
                    AssignedProfile = "No Profile",
                    TargetGameVersion = TargetGameVersion
                },
                SlotNumber = slotIndex + 1,
                Role = "No Session",
                AssignedProfile = "No Profile",
                Level = 0,
                Status = "Offline"
            };

            card.Update(new UnitData
            {
                HealthCurrent = 0,
                HealthMax = 100,
                ManaCurrent = 0,
                ManaMax = 100,
                Level = 0
            });

            return card;
        }

        private int FindFirstNonRealSlotIndex()
        {
            for (int i = 0; i < Fleet.Count; i++)
            {
                if (!Fleet[i].IsRealClient) return i;
            }

            return -1;
        }

        private void ReplaceFleetSlot(int slotIndex, FleetCardViewModel card)
        {
            card.SlotNumber = slotIndex + 1;

            if (slotIndex >= 0 && slotIndex < Fleet.Count)
            {
                Fleet[slotIndex] = card;
            }
            else if (Fleet.Count < MaxFleetSlots)
            {
                Fleet.Add(card);
            }

            OnPropertyChanged(nameof(FleetCountText));
            RefreshSelectionText();
        }

        private BotRole ParseSelectedRole() => ParseRole(SelectedRole);

        private static BotRole ParseRole(string? role)
        {
            return Enum.TryParse(role, ignoreCase: true, out BotRole parsed) ? parsed : BotRole.DPS;
        }

        private void SimulateMockClientTick(FleetCardViewModel card)
        {
            float health = Math.Clamp(card.HealthPercent + _mockRandom.Next(-8, 5), 5, 100);
            float mana = Math.Clamp(card.ManaPercent + _mockRandom.Next(-6, 7), 0, 100);

            card.Update(new UnitData
            {
                HealthCurrent = health,
                HealthMax = 100,
                ManaCurrent = mana,
                ManaMax = 100,
                Level = card.Level > 0 ? card.Level : 80,
                IsDead = health <= 0
            });
        }

        private void EnsureNavigationPlaceholders()
        {
            if (NavigationWaypoints.Count > 0) return;

            NavigationWaypoints.Add("Waypoint 1: awaiting live position");
            NavigationWaypoints.Add("Waypoint 2: awaiting live position");
            NavigationWaypoints.Add("Waypoint 3: awaiting live position");
            SelectedNavigationWaypoint = NavigationWaypoints.FirstOrDefault();
        }

        private void ApplySettingsToControllerState(AppSettings settings)
        {
            CustomWowExecutablePath = settings.CustomWowExecutablePath;
            DefaultRole = NormalizeRole(settings.DefaultRole);
            DefaultProfile = NormalizeProfile(settings.DefaultProfile);
            TargetGameVersion = GameVersionCatalog.Normalize(settings.TargetGameVersion);
            PreferredSensesMode = string.Equals(settings.PreferredSensesMode, "Stealth", StringComparison.OrdinalIgnoreCase) ? "Stealth" : "Power";
            SensesScanIntervalMs = settings.SensesScanIntervalMs;
            StealthPixelGridX = settings.StealthPixelGridX;
            StealthPixelGridY = settings.StealthPixelGridY;
            StealthPixelGridSize = settings.StealthPixelGridSize;
            EnableFlickerSuppression = settings.EnableFlickerSuppression;
            EnableBlackoutCheck = settings.EnableBlackoutCheck;
            UseWindowCapture = settings.UseWindowCapture;
            NavigationMode = string.IsNullOrWhiteSpace(settings.NavigationMode) ? "Follow" : settings.NavigationMode;
            FollowDistance = settings.FollowDistance;
            FormationSpacing = settings.FormationSpacing;
            WaypointRadius = settings.WaypointRadius;
            AutoFollowLeader = settings.AutoFollowLeader;
            AvoidOverlap = settings.AvoidOverlap;
            ShowNavigationPath = settings.ShowNavigationPath;
            LastRouteName = settings.LastRouteName;
            AutoScanOnStartup = settings.AutoScanOnStartup;
            DebugMode = settings.DebugMode;
            LogVerbosity = string.IsNullOrWhiteSpace(settings.LogVerbosity) ? "Normal" : settings.LogVerbosity;
            RememberWindowPlacement = settings.RememberWindowPlacement;
            SelectedRole = DefaultRole;
            SelectedProfile = DefaultProfile;
            SensesModeText = IsPowerModeSelected ? "Power Mode selected" : "Stealth Mode selected";
            OnPropertyChanged(nameof(SensesSummary));
            OnPropertyChanged(nameof(NavigationSummary));
        }

        private string NormalizeRole(string? role)
        {
            string candidate = string.IsNullOrWhiteSpace(role) ? BotRole.DPS.ToString() : role;
            return RoleOptions.Contains(candidate) ? candidate : BotRole.DPS.ToString();
        }

        private string NormalizeProfile(string? profile)
        {
            string candidate = string.IsNullOrWhiteSpace(profile) ? "Basic Rotation" : profile;
            return ProfileOptions.Contains(candidate) ? candidate : "Basic Rotation";
        }

        private static bool IsUsableNumber(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

        private void AddLog(string message)
        {
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            OnPropertyChanged(nameof(LogText));
        }

        private void RefreshSelectionText()
        {
            OnPropertyChanged(nameof(SelectedAvailableClientText));
            OnPropertyChanged(nameof(SelectedFleetCardText));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
