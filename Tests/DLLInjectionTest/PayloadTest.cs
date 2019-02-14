using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSploit.DLLInjection;

namespace SharpSploit.Tests.DLLInjectionTest
{
    [TestClass]
    public class PayloadTests
    {
        private Payload _payload;

        [TestInitialize]
        public void SetUp()
        {
            _payload = Payload.Generate("break;", 7);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void Should_Generate_Payload()
        {
            string content = @"break;";

            // actual
            Payload payload = Payload.Generate(content, 7);

            // expected
            string expected = DLLInjection.Properties.Resources.PayloadSkeleton
                .Replace(Payload.SKELETON_PLACEHOLDER_BODY, content)
                .Replace(Payload.SKELETON_PLACEHOLDER_SLEEP, ((int)TimeSpan.FromSeconds(7).TotalMilliseconds).ToString());

            Assert.AreEqual(expected, payload.Content);
        }

        [TestMethod]
        public void Should_Compile_Valid_Payload()
        {
            FileInfo dll = null;
            try
            {
                dll = _payload.Compile();
            }
            catch (PayloadException)
            {
                Assert.Fail("Exception was thrown during compilation");
            }
            finally
            {
                if (dll != null)
                    dll.Delete();
            }
        }

        [TestMethod]
        public void Should_Not_Compile_Invalid_Payload()
        {
            // generate invalid payload
            string content = @"NOT: VALID; CODE";
            Payload payload = Payload.Generate(content, 7);

            // try to compile
            FileInfo dll = null;
            Assert.ThrowsException<PayloadException>(() => dll = payload.Compile());

            // clean up if successfull
            if (dll != null)
                dll.Delete();
        }
    }
}
