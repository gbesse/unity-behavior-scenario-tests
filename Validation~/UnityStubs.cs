// Compile-only Unity boundary stub; it does not simulate a real Unity graph.
namespace Unity.Behavior {
 public class StubBlackboard { public bool GetVariableValue<T>(string name,out T value){value=default;return false;} }
 public class BehaviorGraphAgent {
  public StubBlackboard BlackboardReference {get;set;}
  public bool SetVariableValue<T>(string name,T value)=>false;
 }
}
