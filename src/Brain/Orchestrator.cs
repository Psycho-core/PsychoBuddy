/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;
using PsychoBuddy.Muscles;

namespace PsychoBuddy.Brain
{
    public class Orchestrator
    {
        private readonly FleetState _fleetState = new FleetState();
        private readonly InputManager _input;
        private readonly Dictionary<int, (RotationEngine Engine, PowerModeSenses Senses)> _botRegistry = new();

        public Orchestrator(InputManager input) => _input = input;

        public void RegisterBot(ClientBinding binding, RotationProfile profile, BotRole role)
        {
            var senses = new PowerModeSenses(binding);
            var engine = new RotationEngine(profile, _input);
            _botRegistry.Add(binding.Pid, (engine, senses));
            _fleetState.MemberRoles.Add(binding.Pid, role);
        }

        public void UpdateFleet()
        {
            foreach (var entry in _botRegistry) _fleetState.MemberStates[entry.Key] = entry.Value.Senses.GetLocalPlayerData();
            foreach (var entry in _botRegistry)
            {
                int pid = entry.Key;
                BotRole role = _fleetState.MemberRoles[pid];
                UnitData? selfData = _fleetState.MemberStates[pid];
                if (role == BotRole.Healer && _fleetState.IsTankDying()) Console.WriteLine($"[Orchestrator] EMERGENCY: Tank dying! Priority Heal for {pid}.");
                entry.Value.Engine.Tick(new IntPtr(0x12345), selfData!);
            }
        }

        public void Shutdown() { foreach (var bot in _botRegistry.Values) bot.Senses.Dispose(); }
    }
}
