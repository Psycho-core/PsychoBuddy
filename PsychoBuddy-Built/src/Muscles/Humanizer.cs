/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using PsychoBuddy.Core;

namespace PsychoBuddy.Muscles
{
    public static class Humanizer
    {
        private static readonly Random _random = new Random();
        public static int GetRandomDelay(int min, int max) => _random.Next(min, max);
        public static List<(int X, int Y)> GenerateBezierPath(int startX, int startY, int endX, int endY)
        {
            List<(int, int)> path = new List<(int, int)>();
            int ctrlX = (startX + endX) / 2 + _random.Next(-50, 50);
            int ctrlY = (startY + endY) / 2 + _random.Next(-50, 50);
            for (double t = 0; t <= 1.0; t += 0.1)
            {
                int x = (int)((1 - t) * (1 - t) * startX + 2 * (1 - t) * t * ctrlX + t * t * endX);
                int y = (int)((1 - t) * (1 - t) * startY + 2 * (1 - t) * t * ctrlY + t * t * endY);
                path.Add((x, y));
            }
            return path;
        }
    }
}
