# Unity Behavior Scenario Tests

Write readable Play Mode tests for [Unity Behavior](https://docs.unity3d.com/jp/current/Manual/com.unity.behavior.html) graphs. `ScenarioRunner` sets blackboard inputs, waits for frames or an expected value, and explains failed assertions. It has no Jev dependency.

```csharp
var scenario = new ScenarioRunner(new AgentBlackboard(agent));
scenario.Set("TargetVisible", true);
yield return scenario.Until("Mode", "Chase", 30);
scenario.Set("TargetVisible", false);
yield return scenario.Until("Mode", "Patrol", 30);
```

Install through Unity Package Manager using the Git URL of this repository, or add `"io.gbesse.behavior-scenario-tests": "file:/absolute/path/to/this/repo"` to your project manifest. The sample in `Samples~/GuardPatrol` shows a Play Mode test; it expects **your** prefab at `Resources/GuardWithBehaviorGraph.prefab` with `TargetVisible` and `Mode` blackboard variables. It is a template, not a bundled game graph. The package tests use a fake blackboard to verify the scenario runner; integration with a user's graph remains a project-level test.

## Français

Écrivez des tests Play Mode lisibles pour les graphes Unity Behavior. `ScenarioRunner` modifie le blackboard, attend un nombre de frames ou une valeur et explique les échecs. L’exemple attend un prefab et deux variables définis dans votre projet ; aucun graphe de jeu n’est fourni.

## Español

Escriba pruebas Play Mode legibles para grafos de Unity Behavior. `ScenarioRunner` modifica el blackboard, espera un número de fotogramas o un valor y explica los fallos. El ejemplo requiere un prefab y dos variables definidos en su proyecto; no se incluye un grafo de juego.

MIT licensed.

## Cancellable frame waits / Attentes annulables / Esperas cancelables

EN: `WaitFrames(count, cancellationToken)` and `Until(name, expected, maxFrames, cancellationToken)` accept an optional `CancellationToken`. Cancellation throws even at a zero-frame deadline, before accepting a result; dispose of the coroutine normally after failure. `Until` checks the final value after exactly the allowed yields, without an inclusive loop that can overflow at `int.MaxValue`. CI executes the actual C# runner with a compile-only Unity boundary stub. Editor tests include cancellation cases, but licensed Unity Editor/Player execution remains unverified. This validates control flow, not a real graph or blackboard API.

FR : `WaitFrames(count, cancellationToken)` et `Until(name, expected, maxFrames, cancellationToken)` acceptent un `CancellationToken` optionnel. L’annulation lève une exception même à zéro frame, avant d’accepter un résultat ; terminez normalement la coroutine après échec. `Until` vérifie la valeur finale après le nombre exact d’attentes autorisées, sans boucle inclusive susceptible de déborder à `int.MaxValue`. Le CI exécute le vrai runner C# avec un substitut Unity de compilation. Les tests Editor incluent l’annulation, mais l’exécution Editor/Player sous licence reste non vérifiée. Cela valide le contrôle, pas un vrai graphe ni l’API blackboard.

ES: `WaitFrames(count, cancellationToken)` y `Until(name, expected, maxFrames, cancellationToken)` aceptan un `CancellationToken` opcional. Cancelar genera una excepción incluso con cero frames, antes de aceptar resultados; finalice normalmente la corrutina tras un fallo. `Until` comprueba el valor final después del número exacto de esperas, sin bucle inclusivo que pueda desbordar en `int.MaxValue`. CI ejecuta el runner C# real con un sustituto Unity solo de compilación. Pruebas Editor incluyen cancelación, pero ejecución Editor/Player licenciada sigue sin verificar. Valida control de flujo, no un grafo real ni API blackboard.
