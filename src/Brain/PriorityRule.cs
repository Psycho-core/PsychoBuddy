/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    /// <summary>
    /// Defines a priority rule: IF (condition) THEN (cast ability).
    /// </summary>
    public class PriorityRule
    {
        public Ability AbilityToCast { get; set; }
        
        // A delegate that takes the current game state and returns true if the rule is met.
        public Func<UnitData, bool> Condition { get; set; }

        public bool Evaluate(UnitData state)
        {
            if (state == null) return false;
            return AbilityToCast.IsReady && Condition(state);
        }
    }
}
