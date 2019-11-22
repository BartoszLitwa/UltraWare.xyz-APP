using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltrwaWare.xyz
{
    public class Injector
    {
        public string Path = "";

        static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        static Injector _instance;

        public static Injector GetInstance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Injector();
                }
                return _instance;
            }
        }

        Injector() { }

        public DllInjectorResult Inject(string ProcName, string DLLPath)
        {
            if (!File.Exists(DLLPath))
                return DllInjectorResult.DLLNotFound;

            uint _procID = 0;

            Process[] _procs = Process.GetProcesses();
            for (int i = 0; i < _procs.Length; i++)
            {
                if (_procs[i].ProcessName == ProcName)
                {
                    _procID = (uint)_procs[i].Id;
                    break;
                }
            }

            if (_procID == 0)
                return DllInjectorResult.GameProcessNotFound;

            if (!bInject(_procID, DLLPath))
                return DllInjectorResult.InjectionFailed;

            return DllInjectorResult.Success;
        }

        public bool bInject(uint ProcToBeInjected, string DLLbytes)
        {
            IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, ProcToBeInjected);

            if (hndProc == INTPTR_ZERO)
                return false;

            IntPtr AllAddresses = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (AllAddresses == INTPTR_ZERO)
                return false;

            IntPtr Address = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)DLLbytes.Length, (0x1000 | 0x2000), 0x40); //(IntPtr)null
            if (Address == INTPTR_ZERO)
                return false;

            byte[] bytes = Encoding.ASCII.GetBytes(DLLbytes);
            if (WriteProcessMemory(hndProc, Address, bytes, (uint)DLLbytes.Length, 0) == 0)
                return false;

            if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, AllAddresses, Address, 0, (IntPtr)null) == INTPTR_ZERO)
                return false;

            CloseHandle(hndProc);
            return true;
        }
    }

    public enum DllInjectorResult
    {
        DLLNotFound,
        GameProcessNotFound,
        InjectionFailed,
        Success,
    }
}
