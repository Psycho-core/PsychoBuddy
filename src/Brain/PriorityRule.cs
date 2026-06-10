/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Brain
{
    public class PriorityRule
    {
        public Ability? AbilityToCast { get; set; }
        public Func<UnitData, bool>? Condition { get; set; }
        public bool Evaluate(UnitData state) => state != null && AbilityToCast != null && Condition != null && AbilityToCast.IsReady && Condition(state);
    }
}
