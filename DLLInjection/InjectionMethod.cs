using System;
using System.IO;

namespace SharpSploit.DLLInjection
{
    public abstract class InjectionMethod
    {
        public abstract void Inject(int pid, FileInfo dll);
    }
}
