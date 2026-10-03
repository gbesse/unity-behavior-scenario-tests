using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BehaviorScenarioTests.Tests
{
    public sealed class ScenarioRunnerTests
    {
        private sealed class Blackboard : IScenarioBlackboard
        {
            private readonly Dictionary<string, object> values = new Dictionary<string, object>();
            public bool Set<T>(string name, T value)
            {
                values[name] = value;
                return true;
            }
            public bool Get<T>(string name, out T value)
            {
                if (values.TryGetValue(name, out var item) && item is T typed)
                {
                    value = typed;
                    return true;
                }
                value = default;
                return false;
            }
        }

        [Test]
        public void SetAndExpectWork()
        {
            var runner = new ScenarioRunner(new Blackboard());
            runner.Set("TargetVisible", true);
            runner.Expect("TargetVisible", true);
        }

        [Test]
        public void FailedExpectationExplainsVariable()
        {
            var runner = new ScenarioRunner(new Blackboard());
            runner.Set("TargetVisible", false);
            Assert.Throws<InvalidOperationException>(() => runner.Expect("TargetVisible", true));
        }

        [Test]
        public void UntilStopsWhenValueAppears()
        {
            var board = new Blackboard();
            var runner = new ScenarioRunner(board);
            var steps = runner.Until("Mode", "Chase", 3);
            Assert.IsTrue(steps.MoveNext());
            board.Set("Mode", "Chase");
            Assert.IsFalse(steps.MoveNext());
        }

        [Test]
        public void UntilFailsAfterDeadline()
        {
            var runner = new ScenarioRunner(new Blackboard());
            var steps = runner.Until("Mode", "Chase", 1);
            Assert.IsTrue(steps.MoveNext());
            Assert.Throws<InvalidOperationException>(() => steps.MoveNext());
        }
    }
}
