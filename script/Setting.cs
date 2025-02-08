using Godot;
using Godot.Collections;
public partial class Setting: Control {
    public Ui ui;
    public OptionButton uiType;
    public OptionButton maxFps;
    /// <summary>
    /// 声音
    /// </summary>
    private Array<Dictionary> voices;
    /// <summary>
    /// 文本转语音的声音选择
    /// </summary>
    public OptionButton tts;
    /// <summary>
    /// 文本转语音的ID
    /// </summary>
    public string ttsId = "";
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
        maxFps = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/maxFps");
        tts = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/tts");
        shadow = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/shadow");
        develop = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/develop");
        useScreenShader = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/useScreenShader");
        showInfo = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/showInfo");
        window = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/window");
        exit = GetNode<Button>("PanelContainer/Scroll/VBoxContainer/exit");
        LOD = GetNode<SpinBox>("PanelContainer/Scroll/VBoxContainer/LOD");
        // 设置初始值
        uiType.Selected = (int) ui.uiType;
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        tts.Selected = 0;
        voices = DisplayServer.TtsGetVoices();
        for (int i = 0; i < voices.Count; i++) {
            ui.Log(voices[i].ToString());
            tts.AddItem(voices[i]["name"].ToString());
        }
        useScreenShader.ButtonPressed = ui.playerCamera.screenShader.Visible;
        light = ui.playerCamera.GetTree().Root.GetChild<Node>(0).GetChild<Node>(0).GetChild<Light3D>(0);
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
        maxFps.ItemSelected += (index) => {
            SetMaxFps();
        };
        tts.ItemSelected += (index) => {
            SetTtsId(index);
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
    public void SetMaxFps() {
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
    }
    public void SetTtsId(long index) {
        if (index == 0) { // 不使用TTS
            ttsId = "";
            return;
        }
        ttsId = voices[((int) index) - 1]["id"].ToString();
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
        ui.playerCamera.screenShader.Visible = useScreenShader.ButtonPressed;
    }
    public void SetShowInfo() {
        ui.infomation.Visible = showInfo.ButtonPressed;
    }
    public void SetWindow() {
        DisplayServer.WindowSetMode(window.ButtonPressed? DisplayServer.WindowMode.Maximized : DisplayServer.WindowMode.ExclusiveFullscreen);
    }
    public void SetWindowVisible() {
        window.Visible = ui.uiType == UiType.computer && develop.ButtonPressed;
    }
    public void SetLOD(double value) {
        ui.GetTree().Root.MeshLodThreshold = (float) value;
    }
}
