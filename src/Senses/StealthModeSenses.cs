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
    public class StealthModeSenses
    {
        private readonly PixelReader _pixelReader;
        public StealthModeSenses(ClientBinding binding) => _pixelReader = new PixelReader(binding.WindowHandle);
        public UnitData GetLocalPlayerData() => StealthTranslator.TranslateGrid(_pixelReader.CaptureGrid());
    }
}
