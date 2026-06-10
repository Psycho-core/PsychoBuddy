/*
========================================================================
SOURCE-AVAILABLE DEVELOPMENT & EVALUATION LICENSE
Copyright (c) 2026 [Psychostout]. All rights reserved.
Refer to LICENSE.MYCODE.txt for full terms.
========================================================================
*/
using System;
using System.Runtime.InteropServices;
using PsychoBuddy.Core;

namespace PsychoBuddy.Senses
{
    public class MemoryReader
    {
        private readonly IntPtr _processHandle;

        public MemoryReader(int pid)
        {
            _processHandle = Win32Api.OpenProcess(
                Win32Api.PROCESS_VM_READ | Win32Api.PROCESS_QUERY_INFORMATION, 
                false, 
                (uint)pid
            );

            if (_processHandle == IntPtr.Zero)
                throw new Exception($"Failed to open process {pid} for memory reading.");
        }

        public void Close()
        {
            Win32Api.CloseHandle(_processHandle);
        }

        public T Read<T>(IntPtr address) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            
            if (!Win32Api.ReadProcessMemory(_processHandle, address, buffer, size, out IntPtr bytesRead))
            {
                return default;
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public IntPtr ReadPointer(IntPtr address)
        {
            return Read<IntPtr>(address);
        }
    }
}
