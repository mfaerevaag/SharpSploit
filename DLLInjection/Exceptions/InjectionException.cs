using System;
using SharpSploit.Core.Exceptions;

namespace SharpSploit.DLLInjection
{
    [Serializable]
    public class InjectionException : SharpSploitException
    {
        public InjectionException(string message) : base(message) { }
    }
}
