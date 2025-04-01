using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Sigil;

namespace SigilTests
{
    [TestFixture]
    public partial class LoadFunctionPointer
    {
        [Test]
        public void CanLoadUnbaked()
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("MethodBuilders"), AssemblyBuilderAccess.Run);
            var mod = asm.DefineDynamicModule("Mod");
            var tb = mod.DefineType("Type");

            var mb = Emit<Action>.BuildInstanceMethod(tb, "UnbakedFunction", MethodAttributes.Public);
            mb.Return();
            var meth = mb.CreateMethod();

            var createProxy = Emit<Func<object>>.BuildStaticMethod(tb, "Create", MethodAttributes.Public | MethodAttributes.Static);
            createProxy.LoadFunctionPointer(meth, new Type[0]);
            createProxy.Return();

            createProxy.CreateMethod();//will throw on failure
        }
    }
}
