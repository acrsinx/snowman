using System.Collections.Generic;
public class TriggerSystem: object {
    public static Dictionary<string, Tool.Void> TriggerList = new();
    public static Dictionary<string, BasicTriggerNode> Conditions = new();
    public static List<string> TriggerNames = new();
    public static void SendTrigger(string triggerName) {
        TriggerNames.Add(triggerName);
        foreach (string key in TriggerList.Keys) {
            BasicTriggerType result = Conditions[key].GetResult();
            switch (result) {
                case BasicTriggerType.alwaysTrue:
                case BasicTriggerType.currentTrue: {
                    TriggerList[key].Invoke();
                    Delete(key);
                    break;
                }
                case BasicTriggerType.alwaysFalse: {
                    Delete(key);
                    break;
                }
                default: {
                    break;
                }
            }
        }
    }
    public static void Delete(string triggerName) {
        TriggerList.Remove(triggerName);
        Conditions.Remove(triggerName);
    }
    public static void AddTrigger(string triggerName, Tool.Void action) {
        if (TriggerList.ContainsKey(triggerName)) {
            TriggerList[triggerName] += action;
        } else {
            TriggerList.Add(triggerName, action);
            Conditions.Add(triggerName, BasicTriggerNode.Parse(triggerName));
        }
    }
}
