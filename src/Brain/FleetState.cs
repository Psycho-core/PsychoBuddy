/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    /// <summary>
    /// The Shared Fleet State (SFS). Centralized data store for all party members.
    /// </summary>
    public class FleetState
    {
        // Maps a Bot's unique ID (PID) to their current game state
        public Dictionary<int, UnitData> MemberStates { get; set; } = new Dictionary<int, UnitData>();
        
        // Maps a Bot's PID to their assigned role
        public Dictionary<int, BotRole> MemberRoles { get; set; } = new Dictionary<int, BotRole>();

        public UnitData GetTankData()
        {
            foreach (var entry in MemberRoles)
            {
                if (entry.Value == BotRole.Tank)
                {
                    MemberStates.TryGetValue(entry.Key, out UnitData data);
                    return data;
                }
            }
            return null;
        }

        public bool IsTankDying()
        {
            var tank = GetTankData();
            return tank != null && tank.HealthPercentage < 40f;
        }
    }
}
