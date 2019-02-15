using System;
using System.IO;

namespace SharpSploit.DLLInjection
{
    public abstract class InjectorMethod
    {
        public abstract IntPtr InjectHandle(IntPtr hProcess, FileInfo dll);
    }
}
