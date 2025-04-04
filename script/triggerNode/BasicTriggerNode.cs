using System.Collections.Generic;
/// <summary>
/// 基本触发节点
/// </summary>
public class BasicTriggerNode {
    public static readonly BasicTriggerNode defaultNode = new();
    public static readonly Dictionary<char, int> CutChar = new() {
        {'(', 1},
        {')', 1},
        {'!', 1},
        {'&', 2},
        {'|', 2},
        {'=', 2}
    };
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
        // 分词，识别"("，")"，"&&"，"||"，"=="，"!"
        for (int i = 0; i < line.Length; i++) {
            char c = line[i];
            if (CutChar.ContainsKey(c)) {
                line = line.Insert(i, " ");
                line = line.Insert(i + CutChar[c] + 1, " ");
                i += CutChar[c] + 1;
            }
        }
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
        BasicTriggerNode left;
        int nextStart = 1;
        // 分级
        if (words[0] == "(") {
            // 找到同级的下一个括号
            int tabLevel = 0;
            for (int i = 1; i < words.Length; i++) {
                if (words[i] == "(") {
                    tabLevel += 1;
                    continue;
                }
                if (words[i] == ")") {
                    tabLevel -= 1;
                    if (tabLevel == 0) {
                        left = Parse(words[1..i]);
                        nextStart = i + 1;
                        break;
                    }
                    continue;
                }
            }
            // 没有找到同级的下一个括号
            return defaultNode;
        } else {
            // 不是括号，直接解析
            left = new GetValueNode(words[0]);
        }
        // 已经结束了
        if (nextStart >= words.Length) {
            return left;
        }
        // 算符
        if (words[1] == "&&") {
            return new TwoValueNode(left, Parse(words[2..]), TwoValueNode.TwoValueOperator.and);
        } else if (words[1] == "||") {
            return new TwoValueNode(left, Parse(words[2..]), TwoValueNode.TwoValueOperator.or);
        } else if (words[1] == "==") {
            return new TwoValueNode(left, Parse(words[2..]), TwoValueNode.TwoValueOperator.equal);
        } else if (words[1] == "!") {
            return new NotNode(left);
        }
        return left;
    }
    public static bool IsTrue(BasicTriggerType type) {
        return type == BasicTriggerType.currentTrue || type == BasicTriggerType.alwaysTrue;
    }
    public static BasicTriggerType GetFromBool(bool value) {
        return value?BasicTriggerType.currentTrue:BasicTriggerType.currentFalse;
    }
}
