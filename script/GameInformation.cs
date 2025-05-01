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
            {
                "totalGameTime", Ui.totalGameTime.ToString()
            }, {
                "vsync", ui.settingPanel.GetNodeCheckButton("vsync").ButtonPressed?"1":"0"
            }, {
                "maxFps", ui.settingPanel.GetNodeOptionButton("maxFps").Selected.ToString()
            }, {
                "tts", ui.settingPanel.GetNodeOptionButton("tts").Selected.ToString()
            }, {
                "shadow", ui.settingPanel.GetNodeCheckButton("shadow").ButtonPressed?"1":"0"
            }, {
                "develop", ui.settingPanel.GetNodeCheckButton("develop").ButtonPressed?"1":"0"
            }, {
                "useScreenShader", ui.settingPanel.GetNodeCheckButton("useScreenShader").ButtonPressed?"1":"0"
            }, {
                "showInfo", ui.settingPanel.GetNodeCheckButton("showInfo").ButtonPressed?"1":"0"
            }, {
                "window", ui.settingPanel.GetNodeCheckButton("window").ButtonPressed?"1":"0"
            }, {
                "local",
                Translation.Locale
            }
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
        Dictionary<string, string> information = null;
        if (file != null) {
            information = (Dictionary<string, string>) Json.ParseString(file.GetAsText());
        }
        Ui.totalGameTime = long.Parse(SafeRead(information, "totalGameTime") ?? "0");
        bool vsync = (SafeRead(information, "vsync") ?? "1") == "1";
        ui.settingPanel.GetNodeCheckButton("vsync").ButtonPressed = vsync;
        ui.settingPanel.SetVsync();
        ui.settingPanel.GetNodeOptionButton("maxFps").Selected = int.Parse(SafeRead(information, "maxFps") ?? "60");
        ui.settingPanel.SetMaxFps();
        int tts = int.Parse(SafeRead(information, "tts") ?? "0");
        ui.settingPanel.GetNodeOptionButton("tts").Selected = tts;
        ui.settingPanel.SetTtsId(tts);
        bool shadow = (SafeRead(information, "shadow") ?? "1") == "1";
        ui.settingPanel.GetNodeCheckButton("shadow").ButtonPressed = shadow;
        ui.settingPanel.SetShadow();
        bool develop = (SafeRead(information, "develop") ?? "0") == "1";
        ui.settingPanel.GetNodeCheckButton("develop").ButtonPressed = develop;
        ui.settingPanel.SetDevelop();
        bool useScreenShader = (SafeRead(information, "useScreenShader") ?? "1") == "1";
        ui.settingPanel.GetNodeCheckButton("useScreenShader").ButtonPressed = useScreenShader;
        ui.settingPanel.SetUseScreenShader();
        bool showInfo = (SafeRead(information, "showInfo") ?? "0") == "1";
        ui.settingPanel.GetNodeCheckButton("showInfo").ButtonPressed = showInfo;
        ui.settingPanel.SetShowInfo();
        bool window = (SafeRead(information, "window") ?? "0") == "1";
        ui.settingPanel.GetNodeCheckButton("window").ButtonPressed = window;
        ui.settingPanel.SetWindow();
        string locale = SafeRead(information, "local") ?? TranslationServer.GetLocale();
        Translation.Locale = locale;
        for (int i = 0; i < ui.settingPanel.GetNodeOptionButton("translation").ItemCount; i++) {
            if (ui.settingPanel.GetNodeOptionButton("translation").GetItemText(i) == locale) {
                ui.settingPanel.GetNodeOptionButton("translation").Selected = i;
                break;
            }
        }
        file?.Close();
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
