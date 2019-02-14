using System;

namespace SharpSploit.Core.Exceptions
{
    [Serializable]
    public class SharpSploitException : Exception
    {
        public SharpSploitException(string message) : base(message) { }
    }
}
