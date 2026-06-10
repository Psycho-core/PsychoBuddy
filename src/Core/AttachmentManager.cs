/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psycho-core]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PsychoBuddy.Core;

namespace PsychoBuddy.Core
{
    public class AttachmentManager
    {
        private readonly string _defaultProcessName = "Wow";
        private string? _customProcessPath = null;

        /// <summary>
        /// Scans the system for running WoW processes and returns a list of potential bindings.
        /// </summary>
        public List<ClientBinding> ScanForClients()
        {
            List<ClientBinding> bindings = new List<ClientBinding>();
            
            // 1. Find processes
            var processes = Process.GetProcesses();
            
            foreach (var proc in processes)
            {
                bool isTarget = false;
                
                // Check by default name
                if (proc.ProcessName.StartsWith(_defaultProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    isTarget = true;
                }
                // Check by custom path if provided. Accessing MainModule can fail for protected/system processes,
                // so this is intentionally guarded to keep scanning safe.
                else if (!string.IsNullOrEmpty(_customProcessPath))
                {
                    try
                    {
                        isTarget = proc.MainModule != null &&
                                   proc.MainModule.FileName.Equals(_customProcessPath, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        isTarget = false;
                    }
                }

                if (isTarget)
                {
                    // 2. Find the window handle associated with this process
                    IntPtr hWnd = FindWindowForProcess(proc.Id);
                    if (hWnd != IntPtr.Zero)
                    {
                        string windowTitle = GetWindowTitle(hWnd);
                        string charName = ExtractCharacterName(windowTitle);

                        bindings.Add(new ClientBinding
                        {
                            Pid = proc.Id,
                            WindowHandle = hWnd,
                            CharacterName = charName,
                            Status = BotStatus.Attached
                        });
                    }
                }
            }

            return bindings;
        }

        public void SetCustomProcessPath(string? path)
        {
            _customProcessPath = path;
        }

        private IntPtr FindWindowForProcess(int pid)
        {
            IntPtr foundHwnd = IntPtr.Zero;
            
            Win32Api.EnumWindows((hWnd, lParam) =>
            {
                uint windowPid;
                Win32Api.GetWindowThreadProcessId(hWnd, out windowPid);
                if (windowPid == pid)
                {
                    foundHwnd = hWnd;
                    return false; // Stop enumerating
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);

            return foundHwnd;
        }

        private string GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            Win32Api.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        private string ExtractCharacterName(string title)
        {
            // Expected format: "World of Warcraft - CharacterName"
            if (string.IsNullOrEmpty(title)) return "Unknown";
            
            int dashIndex = title.IndexOf('-');
            if (dashIndex != -1)
            {
                return title.Substring(dashIndex + 1).Trim();
            }
            
            return title;
        }
    }
}
