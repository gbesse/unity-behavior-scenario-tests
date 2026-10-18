using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Behavior;

namespace BehaviorScenarioTests
{
    public interface IScenarioBlackboard
    {
        bool Set<T>(string name, T value);
        bool Get<T>(string name, out T value);
    }

    public sealed class AgentBlackboard : IScenarioBlackboard
    {
        private readonly BehaviorGraphAgent agent;

        public AgentBlackboard(BehaviorGraphAgent agent)
        {
            this.agent = agent ?? throw new ArgumentNullException(nameof(agent));
        }

        public bool Set<T>(string name, T value) => agent.SetVariableValue(name, value);

        public bool Get<T>(string name, out T value)
        {
            if (agent.BlackboardReference == null)
            {
                value = default;
                return false;
            }
            return agent.BlackboardReference.GetVariableValue(name, out value);
        }
    }

    public sealed class ScenarioRunner
    {
        private readonly IScenarioBlackboard blackboard;

        public ScenarioRunner(IScenarioBlackboard blackboard)
        {
            this.blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
        }

        public void Set<T>(string name, T value)
        {
            if (!blackboard.Set(name, value))
                throw new InvalidOperationException($"FR : variable Blackboard '{name}' absente ou de type différent. EN: Blackboard variable '{name}' was not found or has another type. ES: variable Blackboard '{name}' ausente o de otro tipo.");
        }

        public void Expect<T>(string name, T expected)
        {
            if (!blackboard.Get(name, out T actual))
                throw new InvalidOperationException($"FR : variable Blackboard '{name}' absente ou de type différent. EN: Blackboard variable '{name}' was not found or has another type. ES: variable Blackboard '{name}' ausente o de otro tipo.");
            if (!EqualityComparer<T>.Default.Equals(actual, expected))
                throw new InvalidOperationException($"FR : variable Blackboard '{name}' : attendu '{expected}', obtenu '{actual}'. EN: Blackboard variable '{name}': expected '{expected}', got '{actual}'. ES: variable Blackboard '{name}': esperado '{expected}', obtenido '{actual}'.");
        }

        private static void CheckCancellation(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException("FR : scénario annulé. EN: scenario cancelled. ES: escenario cancelado.", cancellationToken);
        }

        public IEnumerator WaitFrames(int count, CancellationToken cancellationToken = default)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            for (var index = 0; index < count; index++)
            {
                CheckCancellation(cancellationToken);
                yield return null;
            }
            CheckCancellation(cancellationToken);
        }

        public IEnumerator HoldsFor<T>(string name, T expected, int count, CancellationToken cancellationToken = default)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "FR : nombre de frames négatif. EN: negative frame count. ES: cantidad de frames negativa.");
            for (var index = 0; index < count; index++)
            {
                CheckCancellation(cancellationToken);
                Expect(name, expected);
                yield return null;
            }
            CheckCancellation(cancellationToken);
            Expect(name, expected);
        }

        public IEnumerator Until<T>(string name, T expected, int maxFrames, CancellationToken cancellationToken = default)
        {
            if (maxFrames < 0) throw new ArgumentOutOfRangeException(nameof(maxFrames));
            for (var index = 0; index < maxFrames; index++)
            {
                CheckCancellation(cancellationToken);
                if (blackboard.Get(name, out T actual) && EqualityComparer<T>.Default.Equals(actual, expected))
                    yield break;
                yield return null;
            }
            CheckCancellation(cancellationToken);
            Expect(name, expected);
        }
    }
}
