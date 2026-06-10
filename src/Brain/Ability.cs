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

namespace PsychoBuddy.Brain
{
    public class Ability
    {
        public string? Name { get; set; }
        public int SpellID { get; set; }
        public int KeyCode { get; set; }
        public float Cooldown { get; set; }
        public DateTime LastCastTime { get; set; }
        public bool IsReady => (DateTime.Now - LastCastTime).TotalSeconds >= Cooldown;
        public void MarkAsCast() => LastCastTime = DateTime.Now;
    }
}
