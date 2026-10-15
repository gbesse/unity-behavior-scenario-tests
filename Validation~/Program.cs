using BehaviorScenarioTests;
using System.Collections.Generic;
using System.Threading;
class Blackboard : IScenarioBlackboard {
 readonly Dictionary<string,object> values=new();
 public bool Set<T>(string name,T value){values[name]=value;return true;}
 public bool Get<T>(string name,out T value){if(values.TryGetValue(name,out var item)&&item is T typed){value=typed;return true;}value=default;return false;}
}
class Program {
 static void Assert(bool value){if(!value)throw new Exception("assertion failed");}
 static void Reject<T>(Action action) where T:Exception {try{action();}catch(T){return;}throw new Exception("expected failure missing");}
 static void Main(){
  var board=new Blackboard();var runner=new ScenarioRunner(board);
  runner.Set("Mode","Idle");runner.Expect("Mode","Idle");Reject<InvalidOperationException>(()=>runner.Expect("Mode","Chase"));
  var until=runner.Until("Mode","Chase",3);Assert(until.MoveNext());board.Set("Mode","Chase");Assert(!until.MoveNext());
  Assert(!runner.Until("Mode","Chase",0).MoveNext());
  var deadline=runner.Until("missing",true,1);Assert(deadline.MoveNext());Reject<InvalidOperationException>(()=>deadline.MoveNext());
  using var cancel=new CancellationTokenSource();var wait=runner.WaitFrames(3,cancel.Token);Assert(wait.MoveNext());cancel.Cancel();
  Reject<OperationCanceledException>(()=>wait.MoveNext());Reject<OperationCanceledException>(()=>runner.Until("Mode","Chase",0,cancel.Token).MoveNext());
  var frames=runner.WaitFrames(2);Assert(frames.MoveNext());Assert(frames.MoveNext());Assert(!frames.MoveNext());
  Reject<ArgumentOutOfRangeException>(()=>runner.WaitFrames(-1).MoveNext());
  Console.WriteLine("FR : logique C# vérifiée sans hôte Unity. EN: C# logic verified without Unity host. ES: lógica C# verificada sin host Unity.");
 }
}
