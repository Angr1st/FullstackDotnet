using Lib.Shared;

namespace Logic.Tests
{
    public class DelayTests
    {
        [Fact]
        public async void Wait()
        {
            var sut = new Delay(TimeSpan.FromSeconds(0.5));
            var result = await sut.WaitOnce(CancellationToken.None);
            Assert.True(result);
        }

        [Fact]
        public async void DoubleWait()
        {
            var sut = new Delay(TimeSpan.FromSeconds(0.5));
            var result = await sut.WaitOnce(CancellationToken.None);
            Assert.True(result);
            result = await sut.WaitOnce(CancellationToken.None);
            Assert.False(result);
        }
    }
}
