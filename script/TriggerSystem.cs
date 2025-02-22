using System.Collections.Generic;
public class TriggerSystem: object {
    public static Dictionary<string, Tool.Void> TriggerList = new();
    public static bool SendTrigger(string triggerName) {
        if (!TriggerList.ContainsKey(triggerName)) {
            return false;
        }
        TriggerList[triggerName].Invoke();
        return true;
    }
    public static void AddTrigger(string triggerName, Tool.Void action) {
        if (TriggerList.ContainsKey(triggerName)) {
            TriggerList[triggerName] += action;
        } else {
            TriggerList.Add(triggerName, action);
        }
    }
}
