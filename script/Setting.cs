using System.Linq;
using Godot;
using Godot.Collections;
public partial class Setting: Control {
    public Ui ui;
    public Container container;
    public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>> options;
    /// <summary>
    /// 声音
    /// </summary>
    public Array<Dictionary> voices;
    /// <summary>
    /// 声音索引
    /// </summary>
    public int voiceIndex;
    /// <summary>
    /// 文本转语音的ID
    /// </summary>
    public string ttsId = "";
    private Light3D light;
    public void Init() {
        // 获取组件
        container = GetNode<Container>("PanelContainer/Scroll/VBoxContainer");
        // 添加设置
        options = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>> {
            {
                "uiType", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "uiType" },
                    { "type", OptionType.OptionButton },
                    { "items", new Array { "Computer", "Phone" } }
                }
            },{
                "vsync", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "vsync" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "maxFps", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "maxFps" },
                    { "type", OptionType.OptionButton },
                    { "items", new Array {"30", "60", "120", "240", "300", "0" } }
                }
            },{
                "tts", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "tts" },
                    { "type", OptionType.OptionButton },
                    { "items", new Array { "禁用" } }
                }
            },{
                "translation", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "translation" },
                    { "type", OptionType.OptionButton },
                    { "items", new Array { "简体中文" } }
                }
            },{
                "shadow", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "shadow" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "develop", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "develop" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "useScreenShader", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "useScreenShader" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "showInfo", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "showInfo" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "window", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "window" },
                    { "type", OptionType.CheckButton }
                }
            },{
                "exit", new System.Collections.Generic.Dictionary<string, object> {
                    { "name", "exit" },
                    { "type", OptionType.Button }
                }
            }
        };
        SetOptions();
        // 设置文字
        Translation.LangageChanged += () => {
            GetNodeOptionButton("uiType").SetItemText(0, Translation.Translate("计算机"));
            GetNodeOptionButton("uiType").SetItemText(1, Translation.Translate("手机"));
            GetNodeCheckButton("vsync").Text = Translation.Translate("开启垂直同步");
            GetNodeOptionButton("tts").SetItemText(0, Translation.Translate("禁用"));
            GetNodeCheckButton("shadow").Text = Translation.Translate("开启阴影");
            GetNodeCheckButton("develop").Text = Translation.Translate("开发者选项");
            GetNodeCheckButton("useScreenShader").Text = Translation.Translate("使用屏幕着色器");
            GetNodeCheckButton("showInfo").Text = Translation.Translate("开启调试信息");
            GetNodeCheckButton("window").Text = Translation.Translate("窗口模式");
            GetNodeButton("exit").Text = Translation.Translate("退出");
        };
        // 设置初始值
        GetNodeOptionButton("uiType").Selected = (int) ui.uiType;
        Engine.MaxFps = GetNodeOptionButton("maxFps").GetItemText(GetNodeOptionButton("maxFps").GetSelectedId()).ToInt();
        GetNodeOptionButton("tts").Selected = 0;
        voices = DisplayServer.TtsGetVoices();
        for (int i = 0; i < voices.Count; i++) {
            GetNodeOptionButton("tts").AddItem(voices[i]["name"].ToString());
        }
        string[] languages = Translation.GetLanguages();
        for (int i = 0; i < languages.Length; i++) {
            GetNodeOptionButton("translation").AddItem(languages[i]);
        }
        GetNodeCheckButton("useScreenShader").ButtonPressed = ui.player.screenShader.Visible;
        light = ui.GetTree().Root.GetNode<Light3D>("Node/sunLight");
        if (light is null) {
            Ui.Log("找不到灯光。");
        }
        GetNodeCheckButton("shadow").ButtonPressed = light.ShadowEnabled;
        GetNodeCheckButton("showInfo").ButtonPressed = ui.infomation.Visible;
        // 绑定事件
        GetNodeOptionButton("uiType").ItemSelected += (index) => {
            SetUiType(index);
        };
        GetNodeCheckButton("vsync").Pressed += () => {
            SetVsync();
        };
        GetNodeOptionButton("maxFps").ItemSelected += (index) => {
            SetMaxFps();
        };
        GetNodeOptionButton("tts").ItemSelected += (index) => {
            SetTtsId(index);
        };
        GetNodeOptionButton("translation").ItemSelected += (index) => {
            string language = GetNodeOptionButton("translation").GetItemText((int) index);
            if (language == null) {
                return;
            }
            if (language == Translation.Locale) {
                return;
            }
            if (language == "简体中文") {
                Translation.Locale = "";
                return;
            }
            Translation.Locale = language;
        };
        GetNodeCheckButton("shadow").Pressed += () => {
            SetShadow();
        };
        GetNodeCheckButton("develop").Pressed += () => {
            SetDevelop();
        };
        GetNodeCheckButton("useScreenShader").Pressed += () => {
            SetUseScreenShader();
        };
        GetNodeCheckButton("showInfo").Pressed += () => {
            SetShowInfo();
        };
        GetNodeCheckButton("window").Pressed += () => {
            SetWindow();
        };
        GetNodeButton("exit").Pressed += () => {
            ui.Exit();
        };
    }
    public void SetUiType(long index) {
        ui.uiType = (UiType) index;
        SetWindowVisible();
    }
    public void SetVsync() {
        bool enabled = GetNodeCheckButton("vsync").ButtonPressed;
        DisplayServer.VSyncMode mode = enabled?DisplayServer.VSyncMode.Enabled:DisplayServer.VSyncMode.Disabled;
        if (mode == DisplayServer.WindowGetVsyncMode()) {
            return;
        }
        DisplayServer.WindowSetVsyncMode(mode);
        GetNodeOptionButton("maxFps").Visible = !enabled;
    }
    public void SetMaxFps() {
        Engine.MaxFps = GetNodeOptionButton("maxFps").GetItemText(GetNodeOptionButton("maxFps").GetSelectedId()).ToInt();
    }
    public void SetTtsId(long index) {
        if (index == 0) { // 不使用TTS
            voiceIndex = -1;
            ttsId = "";
            return;
        }
        voiceIndex = ((int) index) - 1;
        ttsId = voices[voiceIndex]["id"].ToString();
    }
    public void SetShadow() {
        light.ShadowEnabled = GetNodeCheckButton("shadow").ButtonPressed;
    }
    public void SetDevelop() {
        bool dev = GetNodeCheckButton("develop").ButtonPressed;
        GetNodeCheckButton("useScreenShader").Visible = dev;
        GetNodeCheckButton("showInfo").Visible = dev;
        SetWindowVisible();
    }
    public void SetUseScreenShader() {
        ui.player.screenShader.Visible = GetNodeCheckButton("useScreenShader").ButtonPressed;
    }
    public void SetShowInfo() {
        ui.infomation.Visible = GetNodeCheckButton("showInfo").ButtonPressed;
    }
    public void SetWindow() {
        DisplayServer.WindowMode mode = GetNodeCheckButton("window").ButtonPressed?DisplayServer.WindowMode.Maximized:DisplayServer.WindowMode.ExclusiveFullscreen;
        if (DisplayServer.WindowGetMode() == mode) {
            return;
        }
        DisplayServer.WindowSetMode(mode);
    }
    public void SetWindowVisible() {
        GetNodeCheckButton("window").Visible = ui.uiType == UiType.computer && GetNodeCheckButton("develop").ButtonPressed;
    }
    public void SetOptions() {
        foreach (string key in options.Keys) {
            switch ((OptionType) options[key]["type"]) {
                case OptionType.OptionButton: {
                    OptionButton button = new() {
                        Text = key
                    };
                    options[key].Add("node", button);
                    container.AddChild(button);
                    if (!options[key].ContainsKey("items")) {
                        break;
                    }
                    Array items = (Array) options[key]["items"];
                    foreach (string item in items.Select(v => (string)v)) {
                        button.AddItem(item);
                    }
                    break;
                }
                case OptionType.CheckButton: {
                    CheckButton button = new() {
                        Text = key
                    };
                    options[key].Add("node", button);
                    container.AddChild(button);
                    break;
                }
                case OptionType.Button: {
                    Button button = new() {
                        Text = key
                    };
                    options[key].Add("node", button);
                    container.AddChild(button);
                    break;
                }
                default: {
                    Ui.Log(key, "未知类型");
                    break;
                }
            }
        }
    }
    public OptionButton GetNodeOptionButton(string key) {
        if (!options.ContainsKey(key)) {
            Ui.Log("找不到", key);
            return null;
        }
        if ((OptionType) options[key]["type"] != OptionType.OptionButton) {
            Ui.Log(key, "不是 OptionButton");
            return null;
        }
        if (!options[key].ContainsKey("node")) {
            Ui.Log(key, "没有 node");
            return null;
        }
        return (OptionButton) options[key]["node"];
    }
    public CheckButton GetNodeCheckButton(string key) {
        if (!options.ContainsKey(key)) {
            Ui.Log("找不到", key);
            return null;
        }
        if ((OptionType) options[key]["type"] != OptionType.CheckButton) {
            Ui.Log(key, "不是 CheckButton");
            return null;
        }
        if (!options[key].ContainsKey("node")) {
            Ui.Log(key, "没有 node");
            return null;
        }
        return (CheckButton) options[key]["node"];
    }
    public Button GetNodeButton(string key) {
        if (!options.ContainsKey(key)) {
            Ui.Log("找不到", key);
            return null;
        }
        if (!options[key].ContainsKey("node")) {
            Ui.Log(key, "没有 node");
            return null;
        }
        return (Button) options[key]["node"];
    }
}
