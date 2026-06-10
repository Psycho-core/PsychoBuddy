/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
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
        public static UnitData TranslateGrid(Color[,] grid)
        {
            UnitData data = new UnitData();
            float tr = 0;
            for (int x = 0; x < 5; x++) tr += grid[x, 0].R;
            data.HealthCurrent = (tr / 5f);
            data.HealthMax = 255f;
            return data;
        }
        public static bool IsColorActive(Color p, Color t, int tol = 30) => 
            Math.Abs(p.R - t.R) < tol && Math.Abs(p.G - t.G) < tol && Math.Abs(p.B - t.B) < tol;
    }
}
