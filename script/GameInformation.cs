using Godot;
using Godot.Collections;
/// <summary>
/// 游戏信息类
/// </summary>
public class GameInformation: object {
    public Setting setting;
    public Label gameInformation;
    public SnowCover snowCover;
    public Light3D light;
    private UiType uiType;
    public UiType UiType {
        get => uiType;
        set {
            if (uiType == value) {
                return;
            }
            if (value == UiType.computer) {
                setting.GetTree().Root.Scaling3DScale = 1.0f;
                Engine.PhysicsTicksPerSecond = 60;
                Engine.MaxPhysicsStepsPerFrame = 8;
            } else {
                setting.GetTree().Root.Scaling3DScale = 0.7f;
                Engine.PhysicsTicksPerSecond = 30;
                Engine.MaxPhysicsStepsPerFrame = 4;
            }
            uiType = value;
            setting.SetWindowVisible();
            setting.GetNodeOptionButton("uiType").Selected = (int) uiType;
        }
    }
    private float size;
    public float Size {
        get {
            return size;
        }
        set {
            size = value;
            switch (size) {
                case 0.5f: {
                    setting.GetNodeOptionButton("size").Selected = 0;
                    break;
                }
                case 0.75f: {
                    setting.GetNodeOptionButton("size").Selected = 1;
                    break;
                }
                case 1.0f: {
                    setting.GetNodeOptionButton("size").Selected = 2;
                    break;
                }
                case 1.2f: {
                    setting.GetNodeOptionButton("size").Selected = 3;
                    break;
                }
                case 1.3f: {
                    setting.GetNodeOptionButton("size").Selected = 4;
                    break;
                }
                default: {
                    return;
                }
            }
            setting.GetTree().Root.ContentScaleFactor = size;
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
            switch (maxFps) {
                case 30: {
                    setting.GetNodeOptionButton("maxFps").Selected = 0;
                    break;
                }
                case 60: {
                    setting.GetNodeOptionButton("maxFps").Selected = 1;
                    break;
                }
                case 120: {
                    setting.GetNodeOptionButton("maxFps").Selected = 2;
                    break;
                }
                case 240: {
                    setting.GetNodeOptionButton("maxFps").Selected = 3;
                    break;
                }
                case 300: {
                    setting.GetNodeOptionButton("maxFps").Selected = 4;
                    break;
                }
                case 0: {
                    setting.GetNodeOptionButton("maxFps").Selected = 5;
                    break;
                }
            }
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
    private int snowCoverSubDivide;
    public int SnowCoverSubDivide {
        get {
            return snowCoverSubDivide;
        }
        set {
            snowCoverSubDivide = value;
            switch (snowCoverSubDivide) {
                case 0: {
                    setting.GetNodeOptionButton("snowCoverSubDivide").Selected = 0;
                    break;
                }
                case 31: {
                    setting.GetNodeOptionButton("snowCoverSubDivide").Selected = 1;
                    break;
                }
                case 63: {
                    setting.GetNodeOptionButton("snowCoverSubDivide").Selected = 2;
                    break;
                }
                case 127: {
                    setting.GetNodeOptionButton("snowCoverSubDivide").Selected = 3;
                    break;
                }
                case 255: {
                    setting.GetNodeOptionButton("snowCoverSubDivide").Selected = 4;
                    break;
                }
            }
            if (snowCover == null) {
                return;
            }
            snowCover.SetSubDivide(value);
        }
    }
    private int snowCoverSize;
    public int SnowCoverSize {
        get {
            return snowCoverSize;
        }
        set {
            snowCoverSize = value;
            switch (snowCoverSize) {
                case 4: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 0;
                    break;
                }
                case 64: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 1;
                    break;
                }
                case 128: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 2;
                    break;
                }
                case 256: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 3;
                    break;
                }
                case 512: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 4;
                    break;
                }
                case 1024: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 5;
                    break;
                }
                case 2048: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 6;
                    break;
                }
                case 4096: {
                    setting.GetNodeOptionButton("snowCoverSize").Selected = 7;
                    break;
                }
            }
            if (snowCover == null) {
                return;
            }
            snowCover.SetSnowCoverSize(value);
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
            setting.GetNodeCheckButton("showInfo").Visible = value;
            setting.SetWindowVisible();
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
    public GameInformation(Setting setting) {
        this.setting = setting;
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
        Size = Size;
        Vsync = Vsync;
        MaxFps = MaxFps;
        Tts = Tts;
        Shadow = Shadow;
        SnowCoverSubDivide = SnowCoverSubDivide;
        SnowCoverSize = SnowCoverSize;
        Develop = Develop;
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
