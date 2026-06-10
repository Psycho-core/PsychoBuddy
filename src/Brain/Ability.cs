/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;

namespace PsychoBuddy.Brain
{
    /// <summary>
    /// Defines a specific spell or ability within the rotation.
    /// </summary>
    public class Ability
    {
        public string Name { get; set; }
        public int SpellID { get; set; }
        public int KeyCode { get; set; } // The key bound to this spell in WoW
        public float Cooldown { get; set; }
        public DateTime LastCastTime { get; set; }

        public bool IsReady => (DateTime.Now - LastCastTime).TotalSeconds >= Cooldown;

        public void MarkAsCast()
        {
            LastCastTime = DateTime.Now;
        }
    }
}
