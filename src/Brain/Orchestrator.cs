/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;
using PsychoBuddy.Muscles;

namespace PsychoBuddy.Brain
{
    public class Orchestrator
    {
        private readonly FleetState _fleetState;
        private readonly InputManager _input;
        private readonly Dictionary<int, (RotationEngine Engine, PowerModeSenses Senses)> _botRegistry;

        public Orchestrator(InputManager input)
        {
            _fleetState = new FleetState();
            _input = input;
            _botRegistry = new Dictionary<int, (RotationEngine, PowerModeSenses)>();
        }

        public void RegisterBot(ClientBinding binding, RotationProfile profile, BotRole role)
        {
            var senses = new PowerModeSenses(binding);
            var engine = new RotationEngine(profile, _input);
            
            _botRegistry.Add(binding.Pid, (engine, senses));
            _fleetState.MemberRoles.Add(binding.Pid, role);
        }

        /// <summary>
        /// The main party update loop. Syncs states and executes coordinated actions.
        /// </summary>
        public void UpdateFleet()
        {
            // 1. Sync Phase: Update Shared Fleet State for all bots
            foreach (var entry in _botRegistry)
            {
                int pid = entry.Key;
                var senses = entry.Value.Senses;
                _fleetState.MemberStates[pid] = senses.GetLocalPlayerData();
            }

            // 2. Orchestration Phase: Apply party-wide overrides
            foreach (var entry in _botRegistry)
            {
                int pid = entry.Key;
                var (engine, senses) = entry.Value;
                BotRole role = _fleetState.MemberRoles[pid];
                UnitData selfData = _fleetState.MemberStates[pid];
                
                // We need the HWND for the target window. In a real app, we'd store this in the registry.
                // For this simulation, we are assuming we have the HWND.
                IntPtr hWnd = GetHwndForPid(pid);

                // ROLE OVERRIDE: Healer Triage
                if (role == BotRole.Healer && _fleetState.IsTankDying())
                {
                    Console.WriteLine($"[Orchestrator] EMERGENCY: Tank is dying! Forcing Healer (PID {pid}) to prioritize Tank.");
                    // In a full impl, we would inject a high-priority heal rule into the engine here.
                }

                // Execute the standard rotation based on the updated state
                engine.Tick(hWnd, selfData);
            }
        }

        private IntPtr GetHwndForPid(int pid)
        {
            // This would call the AttachmentManager to get the mapped HWND
            return new IntPtr(0x12345); 
        }

        public void Shutdown()
        {
            foreach (var bot in _botRegistry.Values)
            {
                bot.Senses.Dispose();
            }
        }
    }
}
