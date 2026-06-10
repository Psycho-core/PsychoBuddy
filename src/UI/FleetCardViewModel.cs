/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;

namespace PsychoBuddy.UI
{
    /// <summary>
    /// ViewModel for an individual bot card in the Fleet Monitor.
    /// Implements INotifyPropertyChanged for real-time UI updates.
    /// </summary>
    public class FleetCardViewModel : INotifyPropertyChanged
    {
        private float _healthPercent;
        private float _manaPercent;
        private string _status = "Idle";
        private ClientBinding _binding;

        public ClientBinding Binding { get; set; }

        public string CharacterName => Binding?.CharacterName ?? "Unknown";
        public string Role { get; set; }
        public int Level { get; set; }

        public float HealthPercent 
        { 
            get => _healthPercent; 
            set { _healthPercent = value; OnPropertyChanged(); } 
        }

        public float ManaPercent 
        { 
            get => _manaPercent; 
            set { _manaPercent = value; OnPropertyChanged(); } 
        }

        public string Status 
        { 
            get => _status; 
            set { _status = value; OnPropertyChanged(); } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Update(UnitData data)
        {
            if (data == null) return;
            HealthPercent = data.HealthPercentage;
            ManaPercent = data.ManaPercentage;
        }
    }
}
