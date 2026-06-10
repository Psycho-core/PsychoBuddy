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

        /// <summary>
        /// Returns a random delay based on a Gaussian-like distribution.
        /// </summary>
        /// <param name="min">Minimum delay in ms.</param>
        /// <param name="max">Maximum delay in ms.</param>
        public static int GetRandomDelay(int min, int max)
        {
            // Simple random range for this implementation
            return _random.Next(min, max);
        }

        /// <summary>
        /// Generates a sequence of points representing a Bezier curve for mouse movement.
        /// </summary>
        public static List<(int X, int Y)> GenerateBezierPath(int startX, int startY, int endX, int endY)
        {
            List<(int, int)> path = new List<(int, int)>();
            
            // Create a random control point to make the curve "human"
            int ctrlX = (startX + endX) / 2 + _random.Next(-50, 50);
            int ctrlY = (startY + endY) / 2 + _random.Next(-50, 50);

            for (double t = 0; t <= 1.0; t += 0.1)
            {
                // Quadratic Bezier formula: (1-t)^2*P0 + 2(1-t)*t*P1 + t^2*P2
                int x = (int)((1 - t) * (1 - t) * startX + 2 * (1 - t) * t * ctrlX + t * t * endX);
                int y = (int)((1 - t) * (1 - t) * startY + 2 * (1 - t) * t * ctrlY + t * t * endY);
                path.Add((x, y));
            }

            return path;
        }
    }
}
