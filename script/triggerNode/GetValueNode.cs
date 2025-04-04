public class GetValueNode: BasicTriggerNode {
    public string key = "";
    public GetValueNode(string key) {
        this.key = key;
    }
    public override BasicTriggerType GetResult() {
        bool result = TriggerSystem.TriggerNames.Contains(key);
        return result?BasicTriggerType.alwaysTrue:BasicTriggerType.currentFalse;
    }
}
