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
        public int KeyCode { get; set; } = 0x31;
        public float CooldownSeconds { get; set; } = 1.0f;
        public string Condition { get; set; } = "Always";
        public float Threshold { get; set; }
    }
}
