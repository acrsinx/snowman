using Godot;
using Godot.Collections;
public partial class Setting: Control {
    public Ui ui;
    public OptionButton uiType;
    /// <summary>
    /// 开启垂直同步
    /// </summary>
    public CheckButton vsync;
    public OptionButton maxFps;
    /// <summary>
    /// 声音
    /// </summary>
    public Array<Dictionary> voices;
    /// <summary>
    /// 声音索引
    /// </summary>
    public int voiceIndex;
    /// <summary>
    /// 文本转语音的声音选择
    /// </summary>
    public OptionButton tts;
    /// <summary>
    /// 文本转语音的ID
    /// </summary>
    public string ttsId = "";
    public OptionButton translation;
    public CheckButton shadow;
    public CheckButton develop;
    public CheckButton useScreenShader;
    private Light3D light;
    public CheckButton showInfo;
    /// <summary>
    /// 窗口模式
    /// </summary>
    public CheckButton window;
    public SpinBox LOD;
    public Button exit;
    public void Init() {
        // 获取组件
        uiType = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/uiType");
        vsync = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/vsync");
        maxFps = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/maxFps");
        tts = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/tts");
        translation = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/translation");
        shadow = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/shadow");
        develop = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/develop");
        useScreenShader = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/useScreenShader");
        showInfo = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/showInfo");
        window = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/window");
        exit = GetNode<Button>("PanelContainer/Scroll/VBoxContainer/exit");
        LOD = GetNode<SpinBox>("PanelContainer/Scroll/VBoxContainer/LOD");
        // 设置文字
        Translation.LangageChanged += () => {
            uiType.SetItemText(0, Translation.Translate("计算机"));
            uiType.SetItemText(1, Translation.Translate("手机"));
            vsync.Text = Translation.Translate("开启垂直同步");
            tts.SetItemText(0, Translation.Translate("禁用"));
            shadow.Text = Translation.Translate("开启阴影");
            develop.Text = Translation.Translate("开发者选项");
            useScreenShader.Text = Translation.Translate("使用屏幕着色器");
            showInfo.Text = Translation.Translate("开启调试信息");
            window.Text = Translation.Translate("窗口模式");
            exit.Text = Translation.Translate("退出");
        };
        // 设置初始值
        uiType.Selected = (int) ui.uiType;
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        tts.Selected = 0;
        voices = DisplayServer.TtsGetVoices();
        for (int i = 0; i < voices.Count; i++) {
            ui.Log(voices[i].ToString());
            tts.AddItem(voices[i]["name"].ToString());
        }
        string[] languages = Translation.GetLanguages();
        for (int i = 0; i < languages.Length; i++) {
            translation.AddItem(languages[i]);
        }
        useScreenShader.ButtonPressed = ui.player.screenShader.Visible;
        light = ui.GetTree().Root.GetNode<Light3D>("Node/sunLight");
        if (light is null) {
            ui.Log("找不到灯光。");
        }
        shadow.ButtonPressed = light.ShadowEnabled;
        showInfo.ButtonPressed = ui.infomation.Visible;
        LOD.Value = ui.GetTree().Root.MeshLodThreshold;
        // 绑定事件
        uiType.ItemSelected += (index) => {
            SetUiType(index);
        };
        vsync.Pressed += () => {
            SetVsync();
        };
        maxFps.ItemSelected += (index) => {
            SetMaxFps();
        };
        tts.ItemSelected += (index) => {
            SetTtsId(index);
        };
        translation.ItemSelected += (index) => {
            string language = translation.GetItemText((int) index);
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
        shadow.Pressed += () => {
            SetShadow();
        };
        develop.Pressed += () => {
            SetDevelop();
        };
        useScreenShader.Pressed += () => {
            SetUseScreenShader();
        };
        showInfo.Pressed += () => {
            SetShowInfo();
        };
        window.Pressed += () => {
            SetWindow();
        };
        LOD.ValueChanged += (value) => {
            SetLOD(value);
        };
        exit.Pressed += () => {
            ui.Exit();
        };
    }
    public void SetUiType(long index) {
        ui.uiType = (UiType) index;
        SetWindowVisible();
    }
    public void SetVsync() {
        bool enabled = vsync.ButtonPressed;
        DisplayServer.VSyncMode mode = enabled?DisplayServer.VSyncMode.Enabled:DisplayServer.VSyncMode.Disabled;
        if (mode == DisplayServer.WindowGetVsyncMode()) {
            return;
        }
        DisplayServer.WindowSetVsyncMode(mode);
        maxFps.Visible = !enabled;
    }
    public void SetMaxFps() {
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
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
        light.ShadowEnabled = shadow.ButtonPressed;
    }
    public void SetDevelop() {
        bool dev = develop.ButtonPressed;
        useScreenShader.Visible = dev;
        showInfo.Visible = dev;
        LOD.Visible = dev;
        SetWindowVisible();
    }
    public void SetUseScreenShader() {
        ui.player.screenShader.Visible = useScreenShader.ButtonPressed;
    }
    public void SetShowInfo() {
        ui.infomation.Visible = showInfo.ButtonPressed;
    }
    public void SetWindow() {
        DisplayServer.WindowMode mode = window.ButtonPressed?DisplayServer.WindowMode.Maximized:DisplayServer.WindowMode.ExclusiveFullscreen;
        if (DisplayServer.WindowGetMode() == mode) {
            return;
        }
        DisplayServer.WindowSetMode(mode);
    }
    public void SetWindowVisible() {
        window.Visible = ui.uiType == UiType.computer && develop.ButtonPressed;
    }
    public void SetLOD(double value) {
        ui.GetTree().Root.MeshLodThreshold = (float) value;
    }
}
