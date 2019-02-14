using System;
using SharpSploit.Core.Exceptions;

namespace SharpSploit.DLLInjection
{
    [Serializable]
    public class PayloadException : SharpSploitException
    {
        public PayloadException(string message) : base(message) { }
    }
}
