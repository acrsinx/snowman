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
    public CheckButton useScreenShader;
    public CheckButton shadow;
    private Light3D light;
    public CheckButton showInfo;
    public SpinBox LOD;
    public Button exit;
    public void Init() {
        // 获取组件
        uiType = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/uiType");
        maxFps = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/maxFps");
        tts = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/tts");
        useScreenShader = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/useScreenShader");
        shadow = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/shadow");
        showInfo = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/showInfo");
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
            SetMaxFps(index);
        };
        tts.ItemSelected += (index) => {
            SetTtsId(index);
        };
        useScreenShader.Pressed += () => {
            SetUseScreenShader();
        };
        shadow.Pressed += () => {
            SetShadow();
        };
        showInfo.Pressed += () => {
            SetShowInfo();
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
    }
    public void SetMaxFps(long index) {
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
    }
    public void SetTtsId(long index) {
        if (index == 0) { // 不使用TTS
            ttsId = "";
            return;
        }
        ttsId = voices[((int) index) - 1]["id"].ToString();
    }
    public void SetUseScreenShader() {
        ui.playerCamera.screenShader.Visible = useScreenShader.ButtonPressed;
    }
    public void SetShadow() {
        light.ShadowEnabled = shadow.ButtonPressed;
    }
    public void SetShowInfo() {
        ui.infomation.Visible = showInfo.ButtonPressed;
    }
    public void SetLOD(double value) {
        ui.GetTree().Root.MeshLodThreshold = (float) value;
    }
}
