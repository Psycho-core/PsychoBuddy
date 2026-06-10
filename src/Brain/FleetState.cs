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
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    public class FleetState
    {
        public Dictionary<int, UnitData?> MemberStates { get; set; } = new Dictionary<int, UnitData?>();
        public Dictionary<int, BotRole> MemberRoles { get; set; } = new Dictionary<int, BotRole>();

        public UnitData? GetTankData()
        {
            foreach (var entry in MemberRoles)
            {
                if (entry.Value == BotRole.Tank)
                {
                    MemberStates.TryGetValue(entry.Key, out UnitData? data);
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
