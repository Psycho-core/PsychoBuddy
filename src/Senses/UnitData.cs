/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;

namespace PsychoBuddy.Senses
{
    public class UnitData
    {
        public float HealthCurrent { get; set; }
        public float HealthMax { get; set; }
        public float ManaCurrent { get; set; }
        public float ManaMax { get; set; }
        public int Level { get; set; }
        public float Facing { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public bool IsDead { get; set; }
        public float HealthPercentage => HealthMax > 0 ? (HealthCurrent / HealthMax) * 100f : 0;
        public float ManaPercentage => ManaMax > 0 ? (ManaCurrent / ManaMax) * 100f : 0;
    }
}
