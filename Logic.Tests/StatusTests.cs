using Lib.Shared;

namespace Logic.Tests
{
    public class StatusTests
    {
        [Fact]
        public void OutputIdleIcon()
        {
            var sut = new StatusComponent();
            Assert.Equal("idle", sut.GetStatus());
        }

        [Fact]
        public void ProgressToOnline()
        {
            var sut = new StatusComponent();
            sut.Progress();
            Assert.Equal("online", sut.GetStatus());
        }

        [Fact]
        public void ProgressToFinished()
        {
            var sut = new StatusComponent();
            sut.Progress();
            sut.Progress();
            Assert.Equal("finished", sut.GetStatus());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(1000)]
        public void ProgressMultipleTimes(int count)
        {
            var sut = new StatusComponent();
            for (int i = 0; i < count; i++)
            {
                sut.Progress();
            }
            Assert.Equal("finished", sut.GetStatus());
        }
    }
}