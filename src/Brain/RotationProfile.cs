/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    public class RotationProfile
    {
        public string? ProfileName { get; set; }
        public List<PriorityRule> Rules { get; set; } = new List<PriorityRule>();

        public Ability? GetNextAbility(UnitData state)
        {
            foreach (var rule in Rules) if (rule.Evaluate(state)) return rule.AbilityToCast;
            return null;
        }
    }
}
