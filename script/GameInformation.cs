using Godot;
using Godot.Collections;
/// <summary>
/// 游戏信息类
/// </summary>
public class GameInformation: object {
    public Setting setting;
    public Label gameInformation;
    /// <summary>
    /// 屏幕着色器
    /// </summary>
    public MeshInstance3D screenShader;
    public Light3D light;
    private UiType uiType;
    public UiType UiType {
        get => uiType;
        set {
            uiType = value;
            setting.SetWindowVisible();
        }
    }
    private bool vsync;
    public bool Vsync {
        get {
            return vsync;
        }
        set {
            vsync = value;
            setting.GetNodeCheckButton("vsync").ButtonPressed = value;
            DisplayServer.VSyncMode mode = value?DisplayServer.VSyncMode.Enabled:DisplayServer.VSyncMode.Disabled;
            if (mode == DisplayServer.WindowGetVsyncMode()) {
                return;
            }
            DisplayServer.WindowSetVsyncMode(mode);
            setting.GetNodeOptionButton("maxFps").Visible = !value;
        }
    }
    private int maxFps;
    public int MaxFps {
        get {
            return maxFps;
        }
        set {
            maxFps = value;
            setting.GetNodeOptionButton("maxFps").Selected = value;
            Engine.MaxFps = value;
        }
    }
    private int tts;
    public int Tts {
        get {
            return tts;
        }
        set {
            tts = value;
            setting.GetNodeOptionButton("tts").Selected = value;
            if (value == 0) { // 不使用TTS
                setting.voiceIndex = -1;
                setting.ttsId = "";
                return;
            }
            setting.voiceIndex = value - 1;
            setting.ttsId = setting.voices[setting.voiceIndex]["id"].ToString();
        }
    }
    private bool shadow;
    public bool Shadow {
        get {
            return shadow;
        }
        set {
            shadow = value;
            setting.GetNodeCheckButton("shadow").ButtonPressed = value;
            if (light == null) {
                return;
            }
            light.ShadowEnabled = value;
        }
    }
    private bool develop;
    public bool Develop {
        get {
            return develop;
        }
        set {
            develop = value;
            setting.GetNodeCheckButton("develop").ButtonPressed = value;
            setting.GetNodeCheckButton("useScreenShader").Visible = value;
            setting.GetNodeCheckButton("showInfo").Visible = value;
            setting.SetWindowVisible();
        }
    }
    private bool useScreenShader;
    public bool UseScreenShader {
        get {
            return useScreenShader;
        }
        set {
            useScreenShader = value;
            setting.GetNodeCheckButton("useScreenShader").ButtonPressed = value;
            if (screenShader == null) {
                return;
            }
            screenShader.Visible = value;
        }
    }
    private bool showInfo;
    public bool ShowInfo {
        get {
            return showInfo;
        }
        set {
            showInfo = value;
            setting.GetNodeCheckButton("showInfo").ButtonPressed = value;
            if (gameInformation == null) {
                return;
            }
            gameInformation.Visible = value;
        }
    }
    private bool window;
    public bool Window {
        get {
            return window;
        }
        set {
            window = value;
            setting.GetNodeCheckButton("window").ButtonPressed = value;
            DisplayServer.WindowMode mode = value?DisplayServer.WindowMode.Maximized:DisplayServer.WindowMode.ExclusiveFullscreen;
            if (DisplayServer.WindowGetMode() == mode) {
                return;
            }
            DisplayServer.WindowSetMode(mode);
        }
    }
    public GameInformation(Ui ui) {
        setting = ui.settingPanel;
        gameInformation = ui.infomation;
    }
    public GameInformation(Setting setting, MeshInstance3D screenShader, Label gameInformation) {
        this.setting = setting;
        this.screenShader = screenShader;
        this.gameInformation = gameInformation;
    }
    /// <summary>
    /// 保存游戏信息到指定文件
    /// </summary>
    /// <param name="path">文件路径</param>
    public void SaveInformation(string path) {
        Dictionary<string, string> information = new() {
            {
                "totalGameTime",
                Ui.totalGameTime.ToString()
            }, {
                "vsync",
                Vsync?"1":"0"
            }, {
                "maxFps",
                MaxFps.ToString()
            }, {
                "tts",
                Tts.ToString()
            }, {
                "shadow",
                Shadow?"1":"0"
            }, {
                "develop",
                Develop?"1":"0"
            }, {
                "useScreenShader",
                UseScreenShader?"1":"0"
            }, {
                "showInfo",
                ShowInfo?"1":"0"
            }, {
                "window",
                Window?"1":"0"
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
        Vsync = (SafeRead(information, "vsync") ?? "1") == "1";
        MaxFps = int.Parse(SafeRead(information, "maxFps") ?? "60");
        Tts = int.Parse(SafeRead(information, "tts") ?? "0");
        Shadow = (SafeRead(information, "shadow") ?? "1") == "1";
        Develop = (SafeRead(information, "develop") ?? "0") == "1";
        UseScreenShader = (SafeRead(information, "useScreenShader") ?? "1") == "1";
        ShowInfo = (SafeRead(information, "showInfo") ?? "0") == "1";
        Window = (SafeRead(information, "window") ?? "0") == "1";
        string locale = SafeRead(information, "local") ?? TranslationServer.GetLocale();
        Translation.Locale = locale;
        for (int i = 0; i < setting.GetNodeOptionButton("translation").ItemCount; i++) {
            if (setting.GetNodeOptionButton("translation").GetItemText(i) == locale) {
                setting.GetNodeOptionButton("translation").Selected = i;
                break;
            }
        }
        file?.Close();
    }
    public void Refresh() {
        UiType = UiType;
        Vsync = Vsync;
        MaxFps = MaxFps;
        Tts = Tts;
        Shadow = Shadow;
        Develop = Develop;
        UseScreenShader = UseScreenShader;
        ShowInfo = ShowInfo;
        Window = Window;
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
