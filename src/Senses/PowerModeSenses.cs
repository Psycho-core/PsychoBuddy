/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using PsychoBuddy.Core;

namespace PsychoBuddy.Senses
{
    public class PowerModeSenses
    {
        private readonly MemoryReader _reader;
        private readonly IntPtr _localPlayerBase = new IntPtr(0x12345678);

        public PowerModeSenses(ClientBinding binding)
        {
            _reader = new MemoryReader(binding.Pid);
        }

        public UnitData? GetLocalPlayerData()
        {
            // The LocalPlayer pointer usually points to a Unit structure
            IntPtr unitBase = _reader.ReadPointer(_localPlayerBase);
            if (unitBase == IntPtr.Zero) return null;

            return new UnitData
            {
                HealthCurrent = _reader.Read<float>(unitBase + 0),
                HealthMax = _reader.Read<float>(unitBase + 4),
                ManaCurrent = _reader.Read<float>(unitBase + 8),
                ManaMax = _reader.Read<float>(unitBase + 12),
                Level = _reader.Read<int>(unitBase + 16)
            };
        }

        public void Dispose()
        {
            _reader.Close();
        }
    }
}
