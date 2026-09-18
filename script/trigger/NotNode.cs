public class NotNode: BasicTriggerNode {
    public BasicTriggerNode value;
    public NotNode(BasicTriggerNode value) {
        this.value = value;
    }
    public override BasicTriggerType GetResult() {
        BasicTriggerType result = value.GetResult();
        return result switch {
            BasicTriggerType.alwaysTrue => BasicTriggerType.alwaysFalse,
            BasicTriggerType.currentTrue => BasicTriggerType.currentFalse,
            BasicTriggerType.currentFalse => BasicTriggerType.currentTrue,
            BasicTriggerType.alwaysFalse => BasicTriggerType.alwaysTrue,
            _ => BasicTriggerType.currentFalse
        };
    }
}
