/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PsychoBuddy.Brain;
using PsychoBuddy.Core;
using PsychoBuddy.Muscles;
using PsychoBuddy.Senses;

namespace PsychoBuddy.UI
{
    /// <summary>
    /// Main Controller for the PsychoBuddy Dashboard.
    /// Handles the safe client workflow: Scan -> Configure -> Attach -> Start/Pause/Stop -> Detach.
    /// </summary>
    public class DashboardController : INotifyPropertyChanged
    {
        private readonly Orchestrator _orchestrator;
        private readonly AttachmentManager _attachment;

        private string _statusMessage = "Command Center starting...";
        private string _sensesModeText = "Power Mode selected";
        private ClientBinding? _selectedAvailableClient;
        private FleetCardViewModel? _selectedFleetCard;
        private string _selectedRole = BotRole.DPS.ToString();
        private string _selectedProfile = "Basic Rotation";

        public DashboardController()
        {
            Fleet.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FleetCountText));
            AvailableClients.CollectionChanged += (_, _) => OnPropertyChanged(nameof(AvailableClientCountText));

            var input = new InputManager();
            _orchestrator = new Orchestrator(input);
            _attachment = new AttachmentManager();
        }

        public ObservableCollection<ClientBinding> AvailableClients { get; } = new ObservableCollection<ClientBinding>();
        public ObservableCollection<FleetCardViewModel> Fleet { get; } = new ObservableCollection<FleetCardViewModel>();
        public ObservableCollection<string> LogEntries { get; } = new ObservableCollection<string>();

        public IReadOnlyList<string> RoleOptions { get; } = Enum.GetNames<BotRole>();

        public IReadOnlyList<string> ProfileOptions { get; } = new[]
        {
            "Basic Rotation",
            "Tank Assist",
            "Healer Assist",
            "DPS Assist",
            "Manual Follow"
        };

        public ClientBinding? SelectedAvailableClient
        {
            get => _selectedAvailableClient;
            set
            {
                if (_selectedAvailableClient == value) return;
                _selectedAvailableClient = value;
                OnPropertyChanged();
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

        public string AvailableClientCountText => $"Available clients: {AvailableClients.Count}";
        public string FleetCountText => $"Attached fleet: {Fleet.Count(f => f.IsRealClient)}";
        public string LogText => string.Join(Environment.NewLine, LogEntries);

        public event PropertyChangedEventHandler? PropertyChanged;

        public void InitializeFleet()
        {
            AddLog("PsychoBuddy Command Center initialized.");
            LoadStandbyFleet("Dashboard ready. Scan for clients to begin attachment workflow.");
            RefreshClientScan();
        }

        public void RefreshClientScan()
        {
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

            if (FleetHasOnlyStandbySlots())
            {
                Fleet.Clear();
            }

            BotRole role = ParseSelectedRole();
            var binding = new ClientBinding
            {
                Pid = SelectedAvailableClient.Pid,
                WindowHandle = SelectedAvailableClient.WindowHandle,
                CharacterName = SelectedAvailableClient.CharacterName,
                AssignedProfile = SelectedProfile,
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
                ManaMax = 100
            });

            Fleet.Add(card);
            SelectedFleetCard = card;

            AddLog($"Attached {binding.CharacterName ?? "Unknown"} as {role} using profile '{SelectedProfile}'.");
            StatusMessage = $"Attached {binding.CharacterName ?? "Unknown"}. Press Start Selected when ready.";

            AvailableClients.Remove(SelectedAvailableClient);
            SelectedAvailableClient = AvailableClients.FirstOrDefault();
        }

        public void DetachSelectedFleetCard()
        {
            var card = SelectedFleetCard;
            if (card == null)
            {
                StatusMessage = "No fleet card selected.";
                AddLog("Detach skipped: select an attached fleet card first.");
                return;
            }

            if (!card.IsRealClient)
            {
                Fleet.Remove(card);
                SelectedFleetCard = null;
                AddLog("Removed standby slot.");
                if (Fleet.Count == 0) LoadStandbyFleet();
                return;
            }

            int pid = card.Binding!.Pid;
            _orchestrator.UnregisterBot(pid);
            card.SetStatus(BotStatus.Disconnected, "Detached");
            Fleet.Remove(card);
            SelectedFleetCard = Fleet.FirstOrDefault(f => f.IsRealClient);

            AddLog($"Detached {card.CharacterName} (PID {pid}).");
            StatusMessage = $"Detached {card.CharacterName}.";

            if (!Fleet.Any(f => f.IsRealClient))
            {
                LoadStandbyFleet("No clients attached. Dashboard is in standby/demo mode.");
            }
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
            int started = 0;
            foreach (var card in Fleet.Where(f => f.IsRealClient).ToList())
            {
                if (StartFleetCard(card, quiet: true)) started++;
            }

            StatusMessage = started > 0 ? $"Started {started} attached client(s)." : "No attached clients were started.";
            AddLog(StatusMessage);
        }

        public void StopAllAttached()
        {
            int stopped = 0;
            foreach (var card in Fleet.Where(f => f.IsRealClient).ToList())
            {
                if (StopFleetCard(card, quiet: true)) stopped++;
            }

            StatusMessage = stopped > 0 ? $"Stopped {stopped} attached client(s)." : "No attached clients were stopped.";
            AddLog(StatusMessage);
        }

        public void LoadStandbyFleet(string statusMessage = "Standby/demo fleet slots loaded.")
        {
            if (Fleet.Any(f => f.IsRealClient))
            {
                AddLog("Standby fleet load skipped because real clients are attached.");
                return;
            }

            Fleet.Clear();
            AddStandbySlot("Ares", BotRole.DPS, 100, 50);
            AddStandbySlot("Luna", BotRole.Healer, 100, 100);
            AddStandbySlot("Atlas", BotRole.Tank, 80, 20);
            AddStandbySlot("Nyx", BotRole.DPS, 60, 70);
            SelectedFleetCard = Fleet.FirstOrDefault();
            StatusMessage = statusMessage;
            AddLog("Standby fleet slots loaded.");
        }

        public void Tick()
        {
            try
            {
                _orchestrator.UpdateFleet();

                foreach (var card in Fleet.Where(f => f.IsRealClient && f.Binding != null))
                {
                    UnitData? data = _orchestrator.GetLastUnitData(card.Binding!.Pid);
                    if (data != null) card.Update(data);
                }

                AddLog("Fleet tick processed for running clients.");
            }
            catch (Exception ex)
            {
                AddLog($"Fleet tick skipped: {ex.Message}");
            }
        }

        public void ToggleSensesMode(bool powerMode)
        {
            SensesModeText = powerMode ? "Power Mode selected" : "Stealth Mode selected";
            AddLog($"Senses mode changed: {SensesModeText}.");
        }

        private bool StartFleetCard(FleetCardViewModel card, bool quiet = false)
        {
            if (card.Binding == null || !card.IsRealClient)
            {
                if (!quiet)
                {
                    StatusMessage = "Selected fleet slot is not attached to a real client.";
                    AddLog("Start skipped: standby/demo slots cannot be started.");
                }
                return false;
            }

            try
            {
                BotRole role = ParseRole(card.Role);
                RotationProfile profile = CreateBasicProfile(card.AssignedProfile);

                if (!_orchestrator.IsRegistered(card.Binding.Pid))
                {
                    _orchestrator.RegisterBot(card.Binding, profile, role);
                }

                card.SetStatus(BotStatus.Running, "Running");
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
                AddLog($"Failed to start {card.CharacterName}: {ex.Message}");
                StatusMessage = $"Failed to start {card.CharacterName}. See technical log.";
                return false;
            }
        }

        private bool StopFleetCard(FleetCardViewModel card, bool quiet = false)
        {
            if (card.Binding == null || !card.IsRealClient)
            {
                if (!quiet)
                {
                    StatusMessage = "Selected fleet slot is not attached to a real client.";
                    AddLog("Stop skipped: standby/demo slots are not registered clients.");
                }
                return false;
            }

            _orchestrator.UnregisterBot(card.Binding.Pid);
            card.SetStatus(BotStatus.Stopped, "Stopped");

            if (!quiet)
            {
                AddLog($"Stopped {card.CharacterName}.");
                StatusMessage = $"Stopped {card.CharacterName}.";
            }
            return true;
        }

        private void AddStandbySlot(string name, BotRole role, float healthPercent, float manaPercent)
        {
            var card = new FleetCardViewModel
            {
                Binding = new ClientBinding
                {
                    CharacterName = name,
                    Status = BotStatus.Disconnected,
                    WindowHandle = IntPtr.Zero,
                    Pid = 0,
                    AssignedProfile = "Demo"
                },
                Role = role.ToString(),
                AssignedProfile = "Demo",
                Level = role == BotRole.Healer ? 82 : 85,
                Status = "Demo / Waiting"
            };

            card.Update(new UnitData
            {
                HealthCurrent = healthPercent,
                HealthMax = 100,
                ManaCurrent = manaPercent,
                ManaMax = 100,
                Level = card.Level
            });

            Fleet.Add(card);
        }

        private bool FleetHasOnlyStandbySlots() => Fleet.Count > 0 && Fleet.All(card => !card.IsRealClient);

        private BotRole ParseSelectedRole() => ParseRole(SelectedRole);

        private static BotRole ParseRole(string? role)
        {
            return Enum.TryParse(role, ignoreCase: true, out BotRole parsed) ? parsed : BotRole.DPS;
        }

        private static RotationProfile CreateBasicProfile(string profileName)
        {
            var profile = new RotationProfile { ProfileName = profileName };
            profile.Rules.Add(new PriorityRule
            {
                AbilityToCast = new Ability { Name = "Basic Action", KeyCode = 0x31, Cooldown = 1.0f },
                Condition = _ => true
            });
            return profile;
        }

        private void AddLog(string message)
        {
            LogEntries.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            OnPropertyChanged(nameof(LogText));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
