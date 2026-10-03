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
