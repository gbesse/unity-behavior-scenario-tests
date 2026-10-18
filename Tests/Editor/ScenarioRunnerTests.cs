using System;
using System.Collections.Generic;
using System.Threading;
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
        [Test]
        public void CancellationStopsWaitAndUntil()
        {
            var runner = new ScenarioRunner(new Blackboard());
            using (var cancellation = new CancellationTokenSource())
            {
                var wait = runner.WaitFrames(10, cancellation.Token);
                var until = runner.Until("Mode", "Chase", 10, cancellation.Token);
                Assert.IsTrue(wait.MoveNext());
                Assert.IsTrue(until.MoveNext());
                cancellation.Cancel();
                Assert.Throws<OperationCanceledException>(() => wait.MoveNext());
                Assert.Throws<OperationCanceledException>(() => until.MoveNext());
            }
        }

        [Test]
        public void ZeroFrameDeadlineAndCancellationAreExplicit()
        {
            var runner = new ScenarioRunner(new Blackboard());
            runner.Set("Mode", "Chase");
            Assert.IsFalse(runner.Until("Mode", "Chase", 0).MoveNext());
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.Cancel();
                Assert.Throws<OperationCanceledException>(() => runner.Until("Mode", "Chase", 0, cancellation.Token).MoveNext());
            }
        }

        [Test]
        public void HoldsForChecksEveryResumeIncludingFinalBoundary()
        {
            var board = new Blackboard();
            var runner = new ScenarioRunner(board);
            board.Set("Mode", "Guard");
            var invariant = runner.HoldsFor("Mode", "Guard", 1);
            Assert.IsTrue(invariant.MoveNext());
            board.Set("Mode", "Attack");
            Assert.Throws<InvalidOperationException>(() => invariant.MoveNext());
            Assert.IsFalse(runner.HoldsFor("Mode", "Attack", 0).MoveNext());
        }
    }
}
