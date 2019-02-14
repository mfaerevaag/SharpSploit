using System;
using SharpSploit.DLLInjection.Methods;

namespace SharpSploit.DLLInjection
{
    public class Injection
    {
        private InjectionMethod _injectionMethod;

        private Injection(InjectionMethod method)
        {
            _injectionMethod = method;
        }

        public enum Method
        {
            CREATE_REMOTE_THREAD,
            NT_CREATE_THREAD_EX
        }

        public static InjectionMethod Factory(Method injectionMethod)
        {
            switch (injectionMethod)
            {
                case Method.CREATE_REMOTE_THREAD:
                    return new CreateRemoteThreadMethod();

                case Method.NT_CREATE_THREAD_EX:
                    // return new NtCreateThreadExInjectionStrategy();

                default:
                    throw new NotSupportedException(string.Format("Injection strategy: {0} is not supported", injectionMethod));
            }
        }
    }
}
