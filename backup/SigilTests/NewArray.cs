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
    public partial class NewArray
    {
        [Test]
        public void Simple()
        {
            var e1 = Emit<Func<int[]>>.NewDynamicMethod();
            e1.LoadConstant(128);
            e1.NewArray<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            var x = d1();

            Assert.AreEqual(128, x.Length);
        }
    }
}
