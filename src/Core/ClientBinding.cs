/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;

namespace PsychoBuddy.Core
{
    public enum BotStatus { Disconnected, Attached, Running, Stopped, Paused }

    public class ClientBinding
    {
        public int Pid { get; set; }
        public IntPtr WindowHandle { get; set; }
        public string? CharacterName { get; set; }
        public string? AssignedProfile { get; set; }
        public BotStatus Status { get; set; } = BotStatus.Disconnected;
        public bool IsForeground { get; set; }
        public bool IsMockClient { get; set; }
        public string? TargetGameVersion { get; set; }

        public override string ToString() => IsMockClient
            ? $"[{Status}] {CharacterName ?? "Mock Client"} (Mock)"
            : $"[{Status}] {CharacterName ?? "Unknown"} (PID: {Pid})";
    }
}
