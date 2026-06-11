/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System.Collections.Generic;

namespace PsychoBuddy.Brain
{
    public class ProfileDefinition
    {
        public string Id { get; set; } = "basic-rotation";
        public string DisplayName { get; set; } = "Basic Rotation";
        public string Description { get; set; } = "Starter dashboard profile.";
        public string Role { get; set; } = "DPS";
        public string Version { get; set; } = "Any";
        public string Specialization { get; set; } = "Generic";
        public string Author { get; set; } = "Psycho-core";
        public bool IsEnabled { get; set; } = true;
        public List<string> Tags { get; set; } = new List<string>();
        public List<ProfileRuleDefinition> Rules { get; set; } = new List<ProfileRuleDefinition>();

        public string TagSummary => Tags.Count == 0 ? "No tags" : string.Join(", ", Tags);
        public string MetadataSummary => $"{Role} • {Specialization} • {Version}";
        public string RuleSummary => Rules.Count == 0 ? "No executable rules" : $"{Rules.Count} executable rule(s)";

        public override string ToString() => DisplayName;
    }

    public class ProfileRuleDefinition
    {
        public int Priority { get; set; } = 100;
        public string AbilityName { get; set; } = "Basic Action";
        public int SpellId { get; set; }

        /// <summary>
        /// Win32 virtual key code. 49='1', 50='2', etc. This is the value currently used by InputManager.
        /// </summary>
        public int KeyCode { get; set; } = 0x31;

        /// <summary>
        /// Human-readable keybind label retained for editors/profile UI.
        /// </summary>
        public string KeyBinding { get; set; } = "1";

        public float CooldownSeconds { get; set; } = 1.0f;
        public string Condition { get; set; } = "Always";
        public float Threshold { get; set; }

        /// <summary>
        /// Spell, Buff, Defensive, Interrupt, Movement, Utility, etc.
        /// </summary>
        public string ActionType { get; set; } = "Spell";

        /// <summary>
        /// Enemy, Self, Ally, Party, Ground, None, etc.
        /// </summary>
        public string Target { get; set; } = "Enemy";

        public int MinTargets { get; set; } = 1;
        public int MaxTargets { get; set; } = 1;
        public string RequiredAura { get; set; } = string.Empty;
        public string ForbiddenAura { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
