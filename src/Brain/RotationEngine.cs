/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using PsychoBuddy.Core;
using PsychoBuddy.Senses;
using PsychoBuddy.Muscles;

namespace PsychoBuddy.Brain
{
    public class RotationEngine
    {
        private readonly InputManager _input;
        private readonly RotationProfile _profile;

        public RotationEngine(RotationProfile profile, InputManager input)
        {
            _profile = profile;
            _input = input;
        }

        /// <summary>
        /// Processes one tick of the rotation.
        /// </summary>
        public void Tick(IntPtr hWnd, UnitData state)
        {
            Ability nextAbility = _profile.GetNextAbility(state);

            if (nextAbility != null)
            {
                Console.WriteLine($"[Brain] Priority Match: Casting {nextAbility.Name} (Key: 0x{nextAbility.KeyCode:X})");
                _input.PressKey(hWnd, nextAbility.KeyCode);
                nextAbility.MarkAsCast();
            }
        }
    }
}
