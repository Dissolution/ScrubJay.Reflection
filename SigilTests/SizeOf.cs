using NUnit.Framework;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestFixture]
    public partial class SizeOf
    {
        [Test]
        public void Simple()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.SizeOf<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(sizeof(int), d1());
        }
    }
}
