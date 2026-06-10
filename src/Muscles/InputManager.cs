/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Threading;
using PsychoBuddy.Core;

namespace PsychoBuddy.Muscles
{
    public class InputManager
    {
        /// <summary>
        /// Sends a key-press to a specific window with humanized timing.
        /// </summary>
        public void PressKey(IntPtr hWnd, int keyCode, bool humanize = true)
        {
            // 1. Press the key
            Win32Api.PostMessage(hWnd, Win32Api.WM_KEYDOWN, (IntPtr)keyCode, IntPtr.Zero);

            // 2. Hold the key for a randomized duration (Humanization)
            if (humanize)
            {
                int holdTime = Humanizer.GetRandomDelay(45, 120);
                Thread.Sleep(holdTime);
            }

            // 3. Release the key
            Win32Api.PostMessage(hWnd, Win32Api.WM_KEYUP, (IntPtr)keyCode, IntPtr.Zero);
        }

        /// <summary>
        /// Moves the mouse to a target coordinate using a Bezier curve for humanization.
        /// </summary>
        public void MoveMouse(IntPtr hWnd, int targetX, int targetY, bool humanize = true)
        {
            // In a real background app, we would use PostMessage with WM_MOUSEMOVE.
            // For the simulation, we calculate the humanized path.
            
            if (humanize)
            {
                var path = Humanizer.GenerateBezierPath(0, 0, targetX, targetY); // Simplified start at 0,0
                foreach (var point in path)
                {
                    // Send move message for each point in the curve
                    IntPtr lParam = (IntPtr)((targetY << 16) | (targetX & 0xFFFF));
                    Win32Api.PostMessage(hWnd, 0x0200, IntPtr.Zero, lParam); // WM_MOUSEMOVE = 0x0200
                    Thread.Sleep(Humanizer.GetRandomDelay(5, 15));
                }
            }
            else
            {
                IntPtr lParam = (IntPtr)((targetY << 16) | (targetX & 0xFFFF));
                Win32Api.PostMessage(hWnd, 0x0200, IntPtr.Zero, lParam);
            }
        }
    }
}
