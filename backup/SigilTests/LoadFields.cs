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
    public partial class LoadFields
    {
        public class A
        {
            public int X;

            public static int Y;
        }

        [Test]
        public void Simple()
        {
            var e1 = Emit<Func<A, int>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadField(typeof(A).GetField("X"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(255, d1(new A { X = 255 }));

            var e2 = Emit<Func<int>>.NewDynamicMethod("E2");
            e2.LoadField(typeof(A).GetField("Y"));
            e2.Return();

            var d2 = e2.CreateDelegate();

            A.Y = 31415926;

            Assert.AreEqual(31415926, d2());
        }

        public struct B
        {
            public int X;
        }

        [Test]
        public void ValueType()
        {
            var e1 = Emit<Func<B, int>>.NewDynamicMethod("E1");
            e1.LoadArgumentAddress(0);
            e1.LoadField(typeof(B).GetField("X"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.AreEqual(255, d1(new B { X = 255 }));

            var e2 = Emit<Func<B, int>>.NewDynamicMethod("E2");
            e2.LoadArgument(0);
            e2.LoadField(typeof(B).GetField("X"));
            e2.Return();

            var d2 = e2.CreateDelegate();

            Assert.AreEqual(255, d1(new B { X = 255 }));
        }
    }
}
