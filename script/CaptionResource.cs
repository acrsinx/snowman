using System.Linq;
using Godot.Collections;
public class CaptionResource: object {
    public int id;
    public string type;
    public string actorName;
    public string caption;
    public int time;
    public string startCode;
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
                // 说话角色名
                actorName = (string) dict["actorName"];
                // 说话内容
                caption = (string) dict["caption"];
                // 估算说话时间
                time = caption.Length * 200;
                break;
            }
        }
    }
}
