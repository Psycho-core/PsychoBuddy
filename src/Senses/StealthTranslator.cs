/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Drawing;
using PsychoBuddy.Senses;

namespace PsychoBuddy.Senses
{
    public static class StealthTranslator
    {
        /// <summary>
        /// Translates the 10x10 pixel grid into a UnitData object.
        /// </summary>
        public static UnitData TranslateGrid(Color[,] grid)
        {
            UnitData data = new UnitData();

            // 1. Decode Health (Pixels 0-4, average Red intensity)
            float totalRed = 0;
            for (int x = 0; x < 5; x++) totalRed += grid[x, 0].R;
            data.HealthCurrent = (totalRed / 5f) * 1.0f; // Simplified mapping
            data.HealthMax = 255f;

            // 2. Decode Mana (Pixels 5-9, average Blue intensity)
            float totalBlue = 0;
            for (int x = 5; x < 10; x++) totalBlue += grid[x, 0].B;
            data.ManaCurrent = (totalBlue / 5f) * 1.0f;
            data.ManaMax = 255f;

            // 3. Target Existence (Pixel 10,0) - If the grid was 10x10, we use indices carefully
            // Since the provided grid is 10x10, we map the logic from the spec:
            // Pixel[0,0] = Health, etc.
            
            return data;
        }

        public static bool IsColorActive(Color pixel, Color target, int tolerance = 30)
        {
            return Math.Abs(pixel.R - target.R) < tolerance &&
                   Math.Abs(pixel.G - target.G) < tolerance &&
                   Math.Abs(pixel.B - target.B) < tolerance;
        }
    }
}
