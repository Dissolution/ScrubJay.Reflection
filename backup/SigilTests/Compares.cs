using NUnit.Framework;
using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestFixture]
    public partial class Compares
    {
        [Test]
        public void Equals()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareEqual();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(1, 1));
            Assert.IsFalse(d1(1, 2));
        }

        [Test]
        public void GreaterThan()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(5, 1));
            Assert.IsFalse(d1(1, 1));
        }

        [Test]
        public void LessThan()
        {
            var e1 = Emit<Func<int, int, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.CompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(6, 10));
            Assert.IsFalse(d1(1, 1));
        }

        [Test]
        public void UnsignedCompareGreaterThan()
        {
            var e1 = Emit<Func<uint, uint, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareGreaterThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1(uint.MaxValue, (uint)int.MaxValue));
            Assert.IsFalse(d1(1, 1));
        }

        [Test]
        public void UnsignedCompareLessThan()
        {
            var e1 = Emit<Func<uint, uint, bool>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedCompareLessThan();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.IsTrue(d1((uint)int.MaxValue, uint.MaxValue));
            Assert.IsFalse(d1(1, 1));
        }
    }
}
