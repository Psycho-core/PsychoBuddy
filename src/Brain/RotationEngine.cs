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
using PsychoBuddy.Muscles;

namespace PsychoBuddy.Brain
{
    public class RotationEngine
    {
        private readonly InputManager _input;
        private readonly RotationProfile _profile;
        public RotationEngine(RotationProfile profile, InputManager input) { _profile = profile; _input = input; }
        public void Tick(IntPtr hWnd, UnitData state)
        {
            Ability? next = _profile.GetNextAbility(state);
            if (next != null) { _input.PressKey(hWnd, next.KeyCode); next.MarkAsCast(); }
        }
    }
}
