using NUnit.Framework;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Multiply
    {
        [Test]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Multiply();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.AreEqual(3.14 * 1.59, d1(3.14, 1.59), 0.000001);
        }

        [Test]
        public void OverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.MultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, int>>();
            Assert.AreEqual(3 * 5, d1(3, 5));

            var e2 = Emit.NewDynamicMethod(typeof(double), new[] { typeof(double), typeof(double) }, "E2");
            e2.LoadArgument(0);
            e2.LoadArgument(1);
            e2.MultiplyOverflow();
            e2.Return();

            var d2 = e2.CreateDelegate<Func<double, double, double>>();
            Assert.AreEqual(3.14 * 1.59, d2(3.14, 1.59), 0.000001);
        }

        [Test]
        public void UnsignedOverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedMultiplyOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int, int, int>>();

            Assert.AreEqual(3 * 5, d1(3, 5));

#if !NETCOREAPP
            var e2 = Emit.NewDynamicMethod(typeof(double), new[] { typeof(double), typeof(double) }, "E2");
            e2.LoadArgument(0);
            e2.LoadArgument(1);
            e2.UnsignedMultiplyOverflow();
            e2.Return();

            var d2 = e2.CreateDelegate<Func<double, double, double>>();
            Assert.AreEqual(3.14 * 1.59, d2(3.14, 1.59), 0.000001);
#endif
        }
    }
}
