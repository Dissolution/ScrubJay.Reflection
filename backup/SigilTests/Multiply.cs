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
    public partial class Multiply
    {
        [Test]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Multiply();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(3.14 * 1.59, d1(3.14, 1.59), 0.000001);
        }

        [Test]
        public void Overflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.MultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();
            Assert.AreEqual(3 * 5, d1(3, 5));

            var e2 = Emit<Func<double, double, double>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadArgument(1);
            e2.MultiplyOverflow();
            e2.Return();

            var d2 = e2.CreateDelegate();
            Assert.AreEqual(3.14 * 1.59, d2(3.14, 1.59), 0.000001);
        }

        [Test]
        public void UnsignedOverflow()
        {
            var e1 = Emit<Func<int, int, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedMultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();
            Assert.AreEqual(3 * 5, d1(3, 5));

#if !NETCOREAPP
            var e2 = Emit<Func<double, double, double>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadArgument(1);
            e2.UnsignedMultiplyOverflow();
            e2.Return();

            var d2 = e2.CreateDelegate();
            Assert.AreEqual(3.14 * 1.59, d2(3.14, 1.59), 0.000001);
#endif
        }
    }
}
