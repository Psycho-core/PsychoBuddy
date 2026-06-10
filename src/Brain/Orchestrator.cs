/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using PsychoBuddy.Core;
using PsychoBuddy.Muscles;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    public class Orchestrator
    {
        private readonly FleetState _fleetState = new FleetState();
        private readonly InputManager _input;
        private readonly Dictionary<int, (ClientBinding Binding, RotationEngine Engine, PowerModeSenses Senses)> _botRegistry = new();

        public Orchestrator(InputManager input) => _input = input;

        public bool IsRegistered(int pid) => _botRegistry.ContainsKey(pid);

        public void RegisterBot(ClientBinding binding, RotationProfile profile, BotRole role)
        {
            if (binding.Pid <= 0) throw new ArgumentException("Cannot register a bot without a valid process id.", nameof(binding));
            if (binding.WindowHandle == IntPtr.Zero) throw new ArgumentException("Cannot register a bot without a valid window handle.", nameof(binding));

            if (_botRegistry.ContainsKey(binding.Pid))
            {
                _fleetState.MemberRoles[binding.Pid] = role;
                return;
            }

            var senses = new PowerModeSenses(binding);
            var engine = new RotationEngine(profile, _input);

            _botRegistry[binding.Pid] = (binding, engine, senses);
            _fleetState.MemberRoles[binding.Pid] = role;
        }

        public void UnregisterBot(int pid)
        {
            if (_botRegistry.TryGetValue(pid, out var bot))
            {
                bot.Senses.Dispose();
                _botRegistry.Remove(pid);
            }

            _fleetState.MemberStates.Remove(pid);
            _fleetState.MemberRoles.Remove(pid);
        }

        public UnitData? GetLastUnitData(int pid)
        {
            _fleetState.MemberStates.TryGetValue(pid, out UnitData? data);
            return data;
        }

        public void UpdateFleet()
        {
            foreach (var entry in _botRegistry)
            {
                int pid = entry.Key;
                var bot = entry.Value;

                if (bot.Binding.Status != BotStatus.Running)
                {
                    continue;
                }

                _fleetState.MemberStates[pid] = bot.Senses.GetLocalPlayerData();
            }

            foreach (var entry in _botRegistry)
            {
                int pid = entry.Key;
                var bot = entry.Value;

                if (bot.Binding.Status != BotStatus.Running)
                {
                    continue;
                }

                BotRole role = _fleetState.MemberRoles.TryGetValue(pid, out BotRole storedRole) ? storedRole : BotRole.DPS;
                UnitData? selfData = _fleetState.MemberStates.TryGetValue(pid, out UnitData? data) ? data : null;

                if (selfData == null)
                {
                    continue;
                }

                if (role == BotRole.Healer && _fleetState.IsTankDying())
                {
                    Console.WriteLine($"[Orchestrator] EMERGENCY: Tank dying! Priority Heal for {pid}.");
                }

                bot.Engine.Tick(bot.Binding.WindowHandle, selfData);
            }
        }

        public void Shutdown()
        {
            foreach (var bot in _botRegistry.Values)
            {
                bot.Senses.Dispose();
            }

            _botRegistry.Clear();
            _fleetState.MemberStates.Clear();
            _fleetState.MemberRoles.Clear();
        }
    }
}
