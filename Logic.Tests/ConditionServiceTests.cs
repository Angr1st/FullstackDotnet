using Lib.Shared;

namespace Logic.Tests
{
    public class ConditionServiceTests
    {
        private bool AlwaysTrue() => true;
        private bool AlwaysFalse() => false;

        [Fact]
        public void AlwaysTrueCondition()
        {
            var setByConditionService = false;
            var sut = new ConditionService(() => setByConditionService = true, AlwaysTrue);
            sut.EvaluateConditions();

            Assert.True(setByConditionService);
        }

        [Fact]
        public void AlwaysFalseCondition()
        {
            var setByConditionService = false;
            var sut = new ConditionService(() => setByConditionService = true, AlwaysFalse);
            sut.EvaluateConditions();

            Assert.False(setByConditionService);
        }
    }
}
