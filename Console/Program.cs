using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpSploit.Core;
using SharpSploit.DLLInjection;

namespace SharpSploit.Console
{
    public class Program
    {
        const string dllPath = "C:\\Users\\mfaerevaag\\Dev\\GameHacking-DLLInjection-Unfinished\\Dll_test.dll";

        const string procName = "ac_client";

        static void Main(string[] args)
        {
            //// check dll file
            //if (!File.Exists(dllPath))
            //{
            //    throw new ArgumentException(string.Format("Cannot access DLL: '{0}'", dllPath));
            //}

            //// get process
            //Process[] procList = Process.GetProcessesByName(procName);
            //if (procList.Length == 0)
            //{
            //    Logger.Error("No process '{0}' found", procName);
            //    throw new ArgumentException();
            //}
            //else if (procList.Length > 1)
            //{
            //    Logger.Error("Multiple '{0} processes found", procName);
            //    throw new ArgumentException();
            //}
            //Process proc = procList[0];

            //FileInfo dll = new FileInfo(dllPath);

            //// go
            //Injection.LoadLib(proc.Id, dll);


            // generate payload
            Payload payload = Payload.Generate(@"
                char message[] = ""DLL_INJECTED\r\n"";
                DWORD tmp;
                WriteFile(hStdout, message, sizeof(message), &tmp, NULL);
            ");

            // compile payload to dll
            FileInfo payloadDll = payload.Compile();

            // start victim process
            Process victim = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = false,
                FileName = @"C:\Users\m\Dev\SharpSploit-DLLInjection\Debug\DLLInjector.Victim.exe",
                //FileName = typeof(Tests.Asdf.Program).Assembly.Location,

                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            });

            Injection.LoadLib(victim.Id, payloadDll);

            bool dllInjected = false;

            victim.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null && e.Data.Contains("DLL_INJECTED"))
                {
                    dllInjected = true;
                }
            };
            victim.BeginOutputReadLine();

            var start = DateTime.Now;
            while (!dllInjected)
            {
                // sleep introduces memory barrier
                Thread.Sleep(100);

                if (DateTime.Now.Subtract(start).TotalSeconds > 10)
                {
                    Logger.Error("No answer from victim");
                    break;
                }
            }

            payloadDll.Delete();
        }
    }
}
