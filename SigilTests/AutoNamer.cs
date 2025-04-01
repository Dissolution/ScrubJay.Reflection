using NUnit.Framework;
using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SigilTests
{
    [TestFixture]
    public class AutoNamer
    {
        [Test]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var loc = e1.DeclareLocal<int>();
            var label = e1.DefineLabel();

            e1.LoadConstant(0);
            e1.StoreLocal(loc);
            e1.Branch(label);
            e1.MarkLabel(label);
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1();
        }

        [Test]
        public void NoCollisions()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            var l1 = e1.DefineLabel("_label0");
            var l2 = e1.DefineLabel();

            Assert.AreEqual(2, e1.Labels.Count);
            Assert.IsTrue(e1.Labels.Names.SingleOrDefault(x => x == "_label0") != null);
            Assert.IsTrue(e1.Labels.Names.SingleOrDefault(x => x == "_label1") != null);
        }
    }
}
