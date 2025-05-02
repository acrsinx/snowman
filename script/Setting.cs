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
    public Light3D light;
    public void Init() {
        // 获取组件
        container = GetNode<Container>("PanelContainer/Scroll/VBoxContainer");
        // 添加设置
        options = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>> {
            {
                "uiType", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "uiType"
                    }, {
                        "type",
                        OptionType.OptionButton
                    }, {
                        "items",
                        new Array<string> {
                            "计算机",
                            "手机"
                        }
                    }
                }
            }, {
                "vsync", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "vsync"
                    }, {
                        "text",
                        "开启垂直同步"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "maxFps", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "maxFps"
                    }, {
                        "type",
                        OptionType.OptionButton
                    }, {
                        "items",
                        new Array<string> {
                            "30",
                            "60",
                            "120",
                            "240",
                            "300",
                            "0"
                        }
                    }
                }
            }, {
                "tts", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "tts"
                    }, {
                        "type",
                        OptionType.OptionButton
                    }, {
                        "items",
                        new Array<string> {
                            "禁用"
                        }
                    }
                }
            }, {
                "translation", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "translation"
                    }, {
                        "type",
                        OptionType.OptionButton
                    }, {
                        "items",
                        new Array<string> {
                            "简体中文"
                        }
                    }
                }
            }, {
                "shadow", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "shadow"
                    }, {
                        "text",
                        "开启阴影"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "develop", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "develop"
                    }, {
                        "text",
                        "开发者选项"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "useScreenShader", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "useScreenShader"
                    }, {
                        "text",
                        "使用屏幕着色器"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "showInfo", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "showInfo"
                    }, {
                        "text",
                        "开启调试信息"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "window", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "window"
                    }, {
                        "text",
                        "窗口模式"
                    }, {
                        "type",
                        OptionType.CheckButton
                    }
                }
            }, {
                "exit", new System.Collections.Generic.Dictionary<string, object> {
                    {
                        "name",
                        "exit"
                    }, {
                        "text",
                        "退出"
                    }, {
                        "type",
                        OptionType.Button
                    }
                }
            }
        };
        SetOptions();
        // 设置文字
        Translation.LangageChanged += () => {
            if (options is null) {
                return;
            }
            foreach (string option in options.Keys) {
                if (options[option]["type"] is null) {
                    continue;
                }
                switch (options[option]["type"]) {
                    case OptionType.CheckButton: {
                        GetNodeCheckButton(option).Text = Translation.Translate((string) options[option]["text"]);
                        break;
                    }
                    case OptionType.OptionButton: {
                        for (int i = 0; i < GetNodeOptionButton(option).ItemCount; i++) {
                            GetNodeOptionButton(option).SetItemText(i, Translation.Translate(((Array<string>) options[option]["items"])[i]));
                        }
                        break;
                    }
                    case OptionType.Button: {
                        GetNodeButton(option).Text = Translation.Translate((string) options[option]["text"]);
                        break;
                    }
                }
            }
        };
        // 设置初始值
        GetNodeOptionButton("uiType").Selected = (int) ui.UiType;
        Engine.MaxFps = GetNodeOptionButton("maxFps").GetItemText(GetNodeOptionButton("maxFps").GetSelectedId()).ToInt();
        GetNodeOptionButton("tts").Selected = 0;
        voices = DisplayServer.TtsGetVoices();
        for (int i = 0; i < voices.Count; i++) {
            ((Array<string>) options["tts"]["items"]).Add(voices[i]["name"].ToString() + i.ToString());
        }
        SetOptions();
        string[] languages = Translation.GetLanguages();
        for (int i = 0; i < languages.Length; i++) {
            ((Array<string>) options["translation"]["items"]).Add(languages[i]);
        }
        SetOptions();
        GetNodeCheckButton("useScreenShader").ButtonPressed = ui.player.screenShader.Visible;
        light = ui.GetTree().Root.GetNode<Light3D>("Node/sunLight");
        if (light is null) {
            Ui.Log("找不到灯光。");
        }
        GetNodeCheckButton("shadow").ButtonPressed = light.ShadowEnabled;
        GetNodeCheckButton("showInfo").ButtonPressed = ui.infomation.Visible;
        // 绑定事件
        GetNodeOptionButton("uiType").ItemSelected += (index) => {
            ui.UiType = (UiType) index;
        };
        GetNodeCheckButton("vsync").Pressed += () => {
            ui.gameInformation.Vsync = GetNodeCheckButton("vsync").ButtonPressed;
        };
        GetNodeOptionButton("maxFps").ItemSelected += (index) => {
            ui.gameInformation.MaxFps = GetNodeOptionButton("maxFps").GetItemText(GetNodeOptionButton("maxFps").GetSelectedId()).ToInt();
        };
        GetNodeOptionButton("tts").ItemSelected += (index) => {
            ui.gameInformation.Tts = (int) index;
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
            ui.gameInformation.Shadow = GetNodeCheckButton("shadow").ButtonPressed;
        };
        GetNodeCheckButton("develop").Pressed += () => {
            ui.gameInformation.Develop = GetNodeCheckButton("develop").ButtonPressed;
        };
        GetNodeCheckButton("useScreenShader").Pressed += () => {
            ui.gameInformation.UseScreenShader = GetNodeCheckButton("useScreenShader").ButtonPressed;
        };
        GetNodeCheckButton("showInfo").Pressed += () => {
            ui.gameInformation.ShowInfo = GetNodeCheckButton("showInfo").ButtonPressed;
        };
        GetNodeCheckButton("window").Pressed += () => {
            ui.gameInformation.Window = GetNodeCheckButton("window").ButtonPressed;
        };
        GetNodeButton("exit").Pressed += () => {
            ui.Exit();
        };
    }
    public void SetWindowVisible() {
        GetNodeCheckButton("window").Visible = ui.UiType == UiType.computer && ui.gameInformation.Develop;
    }
    public void SetOptions() {
        // 移除旧有的选项
        foreach (Node child in container.GetChildren()) {
            container.RemoveChild(child);
        }
        foreach (string key in options.Keys) {
            if (!options[key].ContainsKey("type")) {
                Ui.Log(key, "没有类型");
                continue;
            }
            switch ((OptionType) options[key]["type"]) {
                case OptionType.OptionButton: {
                    OptionButton button = new();
                    if (options[key].ContainsKey("node")) {
                        options[key].Remove("node");
                    }
                    options[key].Add("node", button);
                    container.AddChild(button);
                    if (!options[key].ContainsKey("items")) {
                        break;
                    }
                    Array<string> items = (Array<string>) options[key]["items"];
                    foreach (string item in items) {
                        button.AddItem(item);
                    }
                    break;
                }
                case OptionType.CheckButton: {
                    CheckButton button = new();
                    if (options[key].ContainsKey("node")) {
                        options[key].Remove("node");
                    }
                    options[key].Add("node", button);
                    container.AddChild(button);
                    break;
                }
                case OptionType.Button: {
                    Button button = new();
                    if (options[key].ContainsKey("node")) {
                        options[key].Remove("node");
                    }
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
