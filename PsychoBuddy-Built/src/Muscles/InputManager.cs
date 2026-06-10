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
        public void PressKey(IntPtr hWnd, int keyCode, bool humanize = true)
        {
            Win32Api.PostMessage(hWnd, Win32Api.WM_KEYDOWN, (IntPtr)keyCode, IntPtr.Zero);
            if (humanize) Thread.Sleep(Humanizer.GetRandomDelay(45, 120));
            Win32Api.PostMessage(hWnd, Win32Api.WM_KEYUP, (IntPtr)keyCode, IntPtr.Zero);
        }
        public void MoveMouse(IntPtr hWnd, int targetX, int targetY, bool humanize = true)
        {
            if (humanize)
            {
                var path = Humanizer.GenerateBezierPath(0, 0, targetX, targetY);
                foreach (var point in path)
                {
                    IntPtr lParam = (IntPtr)((point.Y << 16) | (point.X & 0xFFFF));
                    Win32Api.PostMessage(hWnd, 0x0200, IntPtr.Zero, lParam);
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
