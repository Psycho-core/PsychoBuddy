/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
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
            }
        }

        public string CharacterName => Binding?.CharacterName ?? "Unknown";
        public string ProcessIdText => Binding?.Pid > 0 ? $"PID {Binding.Pid}" : "No client attached";
        public string WindowHandleText => Binding?.WindowHandle != IntPtr.Zero ? $"HWND 0x{Binding.WindowHandle.ToInt64():X}" : "No window handle";
        public bool IsRealClient => Binding?.Pid > 0 && Binding.WindowHandle != IntPtr.Zero;

        public string Role
        {
            get => _role;
            set
            {
                if (_role == value) return;
                _role = value;
                OnPropertyChanged();
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
            if (Binding != null)
            {
                Binding.Status = status;
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
