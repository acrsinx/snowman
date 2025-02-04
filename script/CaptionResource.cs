using System.Linq;
using Godot.Collections;
public class CaptionResource: object {
    public int id;
    public string actorName;
    public string caption;
    public int time;
    public string startCode;
    public bool canChoose;
    public string[] choose;
    public string endCode;
    public string[] chooseEndCode;
    public Ui ui;
    public CaptionResource(Ui ui, Dictionary dict, int id) {
        this.id = id;
        this.ui = ui;
        // 说话角色名
        actorName = (string) dict["actorName"];
        // 说话内容
        caption = (string) dict["caption"];
        // 估算说话时间
        time = caption.Length * 200;
        // 类型
        string type = (string) dict["type"];
        // 始代码
        startCode = (string) dict["startCode"];
        // 判断是否是选择
        canChoose = type == "choose";
        if (canChoose) { // 如果是选择
            // 获取选择数量
            int chooseLength = 1;
            if (dict.ContainsKey("1")) {
                chooseLength = 2;
            }
            if (dict.ContainsKey("2")) {
                chooseLength = 3;
            }
            choose = new string[chooseLength];
            chooseEndCode = new string[chooseLength];
            for (int i = 0; i < chooseLength; i++) {
                Dictionary chooseDict = (Dictionary) dict[i.ToString()];
                choose[i] = (string) chooseDict.Keys.First();
                chooseEndCode[i] = (string) chooseDict[choose[i]];
            }
        } else { // 如果不是选择
            // 结尾代码
            endCode = (string) dict["endCode"];
        }
    }
}
