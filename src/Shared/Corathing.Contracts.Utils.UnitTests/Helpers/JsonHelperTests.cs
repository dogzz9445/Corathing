using Microsoft.VisualStudio.TestTools.UnitTesting;
using Corathing.Contracts.Utils.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Utils.Helpers.UnitTests
{
    [TestClass()]
    public class JsonHelperTests
    {
        public class TestClassA
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod()]
        public void DeepCopyTest()
        {
            var obj = new TestClassA() { Name = "Test", Age = 10 };

            var obj2 = JsonHelper.DeepCopy(obj, typeof(TestClassA)) as TestClassA;
            Assert.AreEqual(obj2.Name, "Test");
            Assert.AreEqual(obj2.Age, 10);
        }
    }
}
