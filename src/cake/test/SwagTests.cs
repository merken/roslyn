using System;
using Xunit;
using lib;

namespace test
{
    public class SwagTests
    {
        [Fact]
        public void BillGatesTest()
        {
            Assert.Equal("William Gate$$$$$", new BusinessLogic().GetSwag("BILL"));
        }

        [Fact]
        public void ElonMuskTest()
        {
            Assert.Equal("Te$la", new BusinessLogic().GetSwag("ELON"));
        }

        [Fact]
        public void SteveJobs()
        {
            Assert.Equal("0xDEADBEAF", new BusinessLogic().GetSwag("STEVE"));
        }
    }
}
