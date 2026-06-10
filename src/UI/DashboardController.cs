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

        private string _statusMessage = "Command Center starting...";
        private string _sensesModeText = "Power Mode selected";
        private ClientBinding? _selectedAvailableClient;
        private FleetCardViewModel? _selectedFleetCard;
        private string _selectedRole = BotRole.DPS.ToString();
        private string _selectedProfile = "Basic Rotation";

        public DashboardController()
        {
            Fleet.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(FleetCountText));
                RefreshSelectionText();
            };

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
            RefreshClientScan();
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
                _orchestrator.UpdateFleet();

                foreach (var card in runningCards)
                {
                    UnitData? data = _orchestrator.GetLastUnitData(card.Binding!.Pid);
                    if (data != null) card.Update(data);
                }

                StatusMessage = $"Fleet tick processed: {runningCards.Count} running client(s).";
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
                    AddLog("Start skipped: selected slot is empty/offline, not a real attached client.");
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
            if (card.Binding == null || !card.IsRealClient)
            {
                if (!quiet)
                {
                    StatusMessage = "Selected fleet slot is not attached to a real client.";
                    AddLog("Stop skipped: selected slot is empty/offline, not a registered client.");
                }
                return false;
            }

            _orchestrator.UnregisterBot(card.Binding.Pid);
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
                    AssignedProfile = "No Profile"
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
