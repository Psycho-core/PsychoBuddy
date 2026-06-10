/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;
using PsychoBuddy.Brain;
using PsychoBuddy.Muscles;

namespace PsychoBuddy.UI
{
    /// <summary>
    /// Main Controller for the PsychoBuddy Dashboard.
    /// Links the Backend (Brain/Senses) to the Frontend (ViewModel).
    /// </summary>
    public class DashboardController
    {
        public ObservableCollection<FleetCardViewModel> Fleet { get; set; } = new ObservableCollection<FleetCardViewModel>();
        
        private Orchestrator _orchestrator;
        private InputManager _input;
        private AttachmentManager _attachment;

        public DashboardController()
        {
            _input = new InputManager();
            _orchestrator = new Orchestrator(_input);
            _attachment = new AttachmentManager();
        }

        public void InitializeFleet()
        {
            var clients = _attachment.ScanForClients();
            Fleet.Clear();

            foreach (var client in clients)
            {
                // Assign roles (simplified for demo)
                BotRole role = BotRole.DPS;
                if (client.CharacterName.Contains("Atlas")) role = BotRole.Tank;
                else if (client.CharacterName.Contains("Luna")) role = BotRole.Healer;

                RotationProfile profile = new RotationProfile { ProfileName = $"{client.CharacterName}_Profile" };
                profile.Rules.Add(new PriorityRule { 
                    AbilityToCast = new Ability { Name = "Basic", KeyCode = 0x31, Cooldown = 1.0f }, 
                    Condition = (s) => true 
                });

                _orchestrator.RegisterBot(client, profile, role);

                Fleet.Add(new FleetCardViewModel 
                { 
                    Binding = client, 
                    Role = role.ToString(), 
                    Status = "Running" 
                });
            }
        }

        public void Tick()
        {
            _orchestrator.UpdateFleet();
            
            // Sync the UI cards with the latest data from the Orchestrator's FleetState
            // (In a real app, we'd iterate through the SFS and update the ViewModels)
        }

        public void ToggleSensesMode(bool powerMode)
        {
            Console.WriteLine($"Switching to {(powerMode ? "Power" : "Stealth")} Mode...");
            // Logic to swap Senses implementation in the Orchestrator
        }
    }
}
