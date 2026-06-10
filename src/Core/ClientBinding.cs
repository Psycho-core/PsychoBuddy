/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/
using System;

namespace PsychoBuddy.Core
{
    public enum BotStatus
    {
        Disconnected,
        Attached,
        Running,
        Stopped,
        Paused
    }

    /// <summary>
    /// Represents a binding between the bot software and a specific WoW game client.
    /// </summary>
    public class ClientBinding
    {
        public int Pid { get; set; }
        public IntPtr WindowHandle { get; set; }
        public string CharacterName { get; set; }
        public string AssignedProfile { get; set; }
        public BotStatus Status { get; set; } = BotStatus.Disconnected;
        public bool IsForeground { get; set; }

        public override string ToString()
        {
            return $"[{Status}] {CharacterName} (PID: {Pid})";
        }
    }
}
