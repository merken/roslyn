using System;
using Xunit;
using moksy.core;
using moksy.roslyn;
using System.Diagnostics;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestActualDoNotBaseCalled()
        {
            var moksy = new RoslynMoksy();
            var mokked = moksy.GetMokFor<MokMe>();
            mokked.CallBase(false);

            mokked.MokSomething(m => m.DoStuff(), () =>
                Debug.WriteLine("I AM MOKKED"));

            mokked.MokSomething<int, MokMe>(m => m.DoStuff(MokParams.Any<int>()), (i) =>
                Debug.WriteLine("I AM MOKKED " + i));

            mokked.MokSomething<int, string, MokMe>(m => m.DoStuff(MokParams.Any<int>(), MokParams.Any<string>()), (i, a) =>
                Debug.WriteLine("I AM MOKKED " + i + a));

            mokked.DoStuff(100);
            mokked.DoStuff(42, "IS THIS REAL ?");

            Assert.Equal(false, mokked.WasBoolCalled);
        }

        [Fact]
        public void TestActualBaseCalled()
        {
            var moksy = new RoslynMoksy();
            var mokked = moksy.GetMokFor<MokMe>();
            mokked.CallBase(true);

            mokked.MokSomething(m => m.DoStuff(), () =>
                Debug.WriteLine("I AM MOKKED"));

            mokked.DoStuff();

            Assert.Equal(true, mokked.WasBoolCalled);
        }

        [Fact]
        public void TestMockedBaseCalled()
        {
            var moksy = new RoslynMoksy();
            var mokked = moksy.GetMokFor<MokMe>();
            mokked.CallBase(true);

            mokked.MokSomething(m => m.DoStuff(), () =>
            {
                mokked.WasBoolCalled = true;
                Debug.WriteLine("I AM MOKKED");
            });

            mokked.DoStuff();

            Assert.Equal(true, mokked.WasBoolCalled);
        }
    }
}
