using Godot;
using Godot.Collections;
/// <summary>
/// 游戏信息类
/// </summary>
public class GameInformation: object {
    public Ui ui;
    public GameInformation(Ui ui) {
        this.ui = ui;
    }
    /// <summary>
    /// 保存游戏信息到指定文件
    /// </summary>
    /// <param name="path">文件路径</param>
    public void SaveInformation(string path) {
        Dictionary<string, string> information = new() {
            {"totalGameTime", ui.totalGameTime.ToString()},
            {"maxFps", ui.settingPanel.maxFps.Selected.ToString()},
            {"tts", ui.settingPanel.tts.Selected.ToString()},
            {"shadow", ui.settingPanel.shadow.ButtonPressed?"1":"0"},
            {"develop", ui.settingPanel.develop.ButtonPressed?"1":"0"},
            {"useScreenShader", ui.settingPanel.useScreenShader.ButtonPressed?"1":"0"},
            {"showInfo", ui.settingPanel.showInfo.ButtonPressed?"1":"0"},
            {"window", ui.settingPanel.window.ButtonPressed?"1":"0"},
            {"LOD", ui.settingPanel.LOD.Value.ToString()}
        };
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreLine(Json.Stringify(information));
        file.Close();
    }
    /// <summary>
    /// 从指定文件读取游戏信息
    /// </summary>
    /// <param name="path">文件路径</param>
    public void LoadInformation(string path) {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null) {
            ui.Log("读取游戏信息失败，文件不存在");
            return;
        }
        Dictionary<string, string> information = (Dictionary<string, string>) Json.ParseString(file.GetAsText());
        ui.totalGameTime = long.Parse(SafeRead(information, "totalGameTime") ?? "0");
        ui.settingPanel.maxFps.Selected = int.Parse(SafeRead(information, "maxFps") ?? "60");
        ui.settingPanel.SetMaxFps();
        int tts = int.Parse(SafeRead(information, "tts") ?? "0");
        ui.settingPanel.tts.Selected = tts;
        ui.settingPanel.SetTtsId(tts);
        bool shadow = (SafeRead(information, "shadow") ?? "1") == "1";
        ui.settingPanel.shadow.ButtonPressed = shadow;
        ui.settingPanel.SetShadow();
        bool develop = (SafeRead(information, "develop") ?? "0") == "1";
        ui.settingPanel.develop.ButtonPressed = develop;
        ui.settingPanel.SetDevelop();
        bool useScreenShader = (SafeRead(information, "useScreenShader") ?? "1") == "1";
        ui.settingPanel.useScreenShader.ButtonPressed = useScreenShader;
        ui.settingPanel.SetUseScreenShader();
        bool showInfo = (SafeRead(information, "showInfo") ?? "0") == "1";
        ui.settingPanel.showInfo.ButtonPressed = showInfo;
        ui.settingPanel.SetShowInfo();
        bool window = (SafeRead(information, "window") ?? "0") == "1";
        ui.settingPanel.window.ButtonPressed = window;
        ui.settingPanel.SetWindow();
        double lod = double.Parse(SafeRead(information, "LOD") ?? "1");
        ui.settingPanel.LOD.Value = lod;
        ui.settingPanel.SetLOD(lod);
        file.Close();
    }
    public static string SafeRead(Dictionary<string, string> dict, string key) {
        if (dict == null) {
            return null;
        }
        if (dict.ContainsKey(key)) {
            return dict[key];
        }
        return null;
    }
}
