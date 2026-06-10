/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;

namespace PsychoBuddy.UI
{
    public class FleetCardViewModel : INotifyPropertyChanged
    {
        private float _healthPercent;
        private float _manaPercent;
        private string _status = "Idle";
        private string _role = "Unassigned";
        private string _assignedProfile = "None";
        private int _level;
        private int _slotNumber;
        private ClientBinding? _binding;

        public ClientBinding? Binding
        {
            get => _binding;
            set
            {
                if (_binding == value) return;
                _binding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CharacterName));
                OnPropertyChanged(nameof(ProcessIdText));
                OnPropertyChanged(nameof(WindowHandleText));
                OnPropertyChanged(nameof(IsRealClient));
                OnPropertyChanged(nameof(LevelText));
                OnPropertyChanged(nameof(PortraitGlyph));
                OnPropertyChanged(nameof(HealthText));
                OnPropertyChanged(nameof(ManaText));
            }
        }

        public int SlotNumber
        {
            get => _slotNumber;
            set
            {
                if (_slotNumber == value) return;
                _slotNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SlotLabel));
            }
        }

        public string SlotLabel => SlotNumber > 0 ? $"Slot {SlotNumber}" : "Fleet Slot";
        public string CharacterName => Binding?.CharacterName ?? "Unknown";
        public string ProcessIdText
        {
            get
            {
                ClientBinding? binding = Binding;
                return binding != null && binding.Pid > 0 ? $"PID {binding.Pid}" : "No client attached";
            }
        }

        public string WindowHandleText
        {
            get
            {
                ClientBinding? binding = Binding;
                if (binding == null || binding.WindowHandle == IntPtr.Zero)
                {
                    return "No window handle";
                }

                return $"HWND 0x{binding.WindowHandle.ToInt64():X}";
            }
        }

        public bool IsRealClient
        {
            get
            {
                ClientBinding? binding = Binding;
                return binding != null && binding.Pid > 0 && binding.WindowHandle != IntPtr.Zero;
            }
        }
        public string LevelText => IsRealClient ? "CLIENT" : "OFFLINE";
        public string HealthText => IsRealClient ? $"{HealthPercent:0}%" : "—";
        public string ManaText => IsRealClient ? $"{ManaPercent:0}%" : "—";
        public string PortraitGlyph => IsRealClient ? "◉" : "◇";
        public string RoleIcon => Role.ToLowerInvariant() switch
        {
            "tank" => "🛡",
            "healer" => "✚",
            "dps" => "⚔",
            "utility" => "◆",
            _ => "◇"
        };

        public string Role
        {
            get => _role;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? "Unassigned" : value;
                if (_role == next) return;
                _role = next;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RoleIcon));
                OnPropertyChanged(nameof(PortraitGlyph));
            }
        }

        public string AssignedProfile
        {
            get => _assignedProfile;
            set
            {
                string next = string.IsNullOrWhiteSpace(value) ? "None" : value;
                if (_assignedProfile == next) return;
                _assignedProfile = next;
                OnPropertyChanged();
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                if (_level == value) return;
                _level = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LevelText));
            }
        }

        public float HealthPercent
        {
            get => _healthPercent;
            set
            {
                if (_healthPercent.Equals(value)) return;
                _healthPercent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HealthText));
            }
        }

        public float ManaPercent
        {
            get => _manaPercent;
            set
            {
                if (_manaPercent.Equals(value)) return;
                _manaPercent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ManaText));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetStatus(BotStatus status, string? uiStatus = null)
        {
            ClientBinding? binding = Binding;
            if (binding != null)
            {
                binding.Status = status;
            }

            Status = uiStatus ?? status.ToString();
        }

        public void Update(UnitData? data)
        {
            if (data == null) return;
            HealthPercent = data.HealthPercentage;
            ManaPercent = data.ManaPercentage;
            Level = data.Level;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
