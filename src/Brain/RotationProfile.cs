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
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    /// <summary>
    /// A collection of priority rules for a specific class/spec.
    /// </summary>
    public class RotationProfile
    {
        public string ProfileName { get; set; }
        public List<PriorityRule> Rules { get; set; } = new List<PriorityRule>();

        public Ability GetNextAbility(UnitData state)
        {
            // Evaluate rules in order. The first rule that evaluates to true wins.
            foreach (var rule in Rules)
            {
                if (rule.Evaluate(state))
                {
                    return rule.AbilityToCast;
                }
            }
            return null; // No valid ability to cast
        }
    }
}
