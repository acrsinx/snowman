public class TwoValueNode: BasicTriggerNode {
    public enum TwoValueOperator {
        and,
        or,
        equal
    };
    public BasicTriggerNode value1;
    public BasicTriggerNode value2;
    public TwoValueOperator op;
    public TwoValueNode(BasicTriggerNode value1, BasicTriggerNode value2, TwoValueOperator op) {
        this.value1 = value1;
        this.value2 = value2;
        this.op = op;
    }
    public override BasicTriggerType GetResult() {
        switch (op) {
            case TwoValueOperator.and: {
                BasicTriggerType v1 = value1.GetResult();
                if (v1 == BasicTriggerType.alwaysFalse) {
                    return BasicTriggerType.alwaysFalse;
                }
                BasicTriggerType v2 = value2.GetResult();
                if (v2 == BasicTriggerType.alwaysFalse) {
                    return BasicTriggerType.alwaysFalse;
                }
                if (v1 == BasicTriggerType.alwaysTrue && v2 == BasicTriggerType.alwaysTrue) {
                    return BasicTriggerType.alwaysTrue;
                }
                return GetFromBool(IsTrue(v1) && IsTrue(v2));
            }
            case TwoValueOperator.or: {
                BasicTriggerType v1 = value1.GetResult();
                if (v1 == BasicTriggerType.alwaysTrue) {
                    return BasicTriggerType.alwaysTrue;
                }
                BasicTriggerType v2 = value2.GetResult();
                if (v2 == BasicTriggerType.alwaysTrue) {
                    return BasicTriggerType.alwaysTrue;
                }
                if (v1 == BasicTriggerType.alwaysFalse && v2 == BasicTriggerType.alwaysFalse) {
                    return BasicTriggerType.alwaysFalse;
                }
                return GetFromBool(IsTrue(v1) || IsTrue(v2));
            }
            case TwoValueOperator.equal: {
                return GetFromBool(value1.GetResult() == value2.GetResult());
            }
            default: {
                return BasicTriggerType.currentFalse;
            }
        }
    }
}
