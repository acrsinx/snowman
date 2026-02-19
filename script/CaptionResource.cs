using System.Linq;
using Godot.Collections;
public class CaptionResource: object {
    public int id;
    public string type;
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
        // 类型
        type = (string) dict["type"];
        // 始代码
        startCode = (string) dict["startCode"];
        // 判断类型
        switch (type) {
            case "caption": { // 普通对话
                canChoose = false;
                // 说话角色名
                actorName = (string) dict["actorName"];
                // 说话内容
                caption = (string) dict["caption"];
                // 估算说话时间
                time = caption.Length * 200;
                // 结尾代码
                endCode = (string) dict["endCode"];
                break;
            }
            case "choose": { // 选择
                canChoose = true;
                // 说话角色名
                actorName = (string) dict["actorName"];
                // 说话内容
                caption = (string) dict["caption"];
                // 估算说话时间
                time = caption.Length * 200;
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
                break;
            }
            case "shot": { // 无对话的镜头
                canChoose = false;
                // 时间
                time = (int) dict["time"];
                // 结尾代码
                endCode = (string) dict["endCode"];
                break;
            }
        }
    }
}
