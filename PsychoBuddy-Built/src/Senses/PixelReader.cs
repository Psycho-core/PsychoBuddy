/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Drawing;
using PsychoBuddy.Core;

namespace PsychoBuddy.Senses
{
    public class PixelReader
    {
        private readonly IntPtr _hWnd;
        public PixelReader(IntPtr hWnd) => _hWnd = hWnd;
        public Color[,] CaptureGrid()
        {
            Color[,] grid = new Color[10, 10];
            Random rand = new Random();
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    grid[x, y] = (x < 5) ? Color.FromArgb(rand.Next(100, 255), 0, 0) : Color.Black;
            return grid;
        }
    }
}
