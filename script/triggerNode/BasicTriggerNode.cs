/// <summary>
/// 基本触发节点
/// </summary>
public class BasicTriggerNode {
    public static readonly BasicTriggerNode defaultNode = new();
    /// <summary>
    /// 判断条件是否成立
    /// </summary>
    /// <returns>是否成立</returns>
    public virtual BasicTriggerType GetResult() {
        return BasicTriggerType.currentFalse;
    }
    /// <summary>
    /// 解析
    /// </summary>
    /// <param name="line">要解析的字符串</param>
    /// <returns>解析后的节点</returns>
    public static BasicTriggerNode Parse(string line) {
        // 分词
        string[] words = line.Split(' ');
        if (words.Length == 0) {
            return defaultNode;
        }
        // 去掉空白字符
        for (int i = 0; i < words.Length; i++) {
            words[i] = words[i].Trim();
        }
        return Parse(words);
    }
    public static BasicTriggerNode Parse(string[] words) {
        // 分级
        BasicTriggerNode temp = new GetValueNode(words[0]);
        if (words[1] == "&&") {
            temp = new TwoValueNode(temp, Parse(words[2..]), TwoValueNode.TwoValueOperator.and);
        } else if (words[1] == "||") {
            temp = new TwoValueNode(temp, Parse(words[2..]), TwoValueNode.TwoValueOperator.or);
        } else if (words[1] == "==") {
            temp = new TwoValueNode(temp, Parse(words[2..]), TwoValueNode.TwoValueOperator.equal);
        }
        return temp;
    }
    public static bool IsTrue(BasicTriggerType type) {
        return type == BasicTriggerType.currentTrue || type == BasicTriggerType.alwaysTrue;
    }
    public static BasicTriggerType GetFromBool(bool value) {
        return value?BasicTriggerType.currentTrue:BasicTriggerType.currentFalse;
    }
}
