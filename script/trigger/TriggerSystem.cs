using System.Collections.Generic;
public class TriggerSystem: object {
    public struct Trigger {
        public string trigger;
        public Tool.Void action;
        public BasicTriggerNode node;
        public Trigger(string trigger, Tool.Void action, BasicTriggerNode node) {
            this.trigger = trigger;
            this.action = action;
            this.node = node;
        }
        public Trigger(string trigger, Tool.Void action) {
            this.trigger = trigger;
            this.action = action;
            node = null;
        }
        public static Trigger operator + (Trigger a, Tool.Void b) {
            return new Trigger(a.trigger, () => {
                a.action();
                b();
            }, a.node);
        }
    }
    private static readonly List<Trigger> TriggerList = new();
    private static readonly List<Trigger> toAddTriggerList = new();
    public static readonly List<string> TriggerNames = new();
    public static void SendTrigger(string triggerName) {
        if (TriggerNames.Contains(triggerName)) {
            return;
        }
        TriggerNames.Add(triggerName);
        CheckAll();
    }
    public static void SendTriggerCurrent(string triggerName) {
        TriggerNames.Add(triggerName);
        CheckAll();
        TriggerNames.RemoveAt(TriggerNames.Count - 1);
    }
    public static void CheckAll() {
        foreach (Trigger trigger in toAddTriggerList) {
            RealAddTrigger(trigger.trigger, trigger.action);
        }
        toAddTriggerList.Clear();
        int i = 0;
        while (i < TriggerList.Count) {
            BasicTriggerType result = TriggerList[i].node.GetResult();
            switch (result) {
                case BasicTriggerType.alwaysTrue:
                case BasicTriggerType.currentTrue: {
                    Tool.Void toInvoke = TriggerList[i].action;
                    Delete(i);
                    toInvoke();
                    break;
                }
                case BasicTriggerType.alwaysFalse: {
                    Delete(i);
                    break;
                }
                default: {
                    i++;
                    break;
                }
            }
        }
    }
    private static void Delete(int i) {
        TriggerList.RemoveAt(i);
    }
    public static void AddTrigger(string triggerName, Tool.Void action) {
        toAddTriggerList.Add(new Trigger(triggerName, action));
    }
    private static void RealAddTrigger(string triggerName, Tool.Void action) {
        int index = TriggerList.FindIndex(tr => tr.trigger == triggerName);
        if (index == -1) {
            TriggerList.Add(new Trigger(triggerName, action, BasicTriggerNode.Parse(triggerName)));
        } else {
            TriggerList[index] += action;
        }
    }
}
