using Godot;

public partial class Setting : Control {
    public Ui ui;
    public OptionButton uiType;
    public OptionButton maxFps;
    public CheckButton useScreenShader;
    public CheckButton shadow;
    private Light3D light;
    public CheckButton showInfo;
    private SpinBox LOD;
    public Button exit;
    public void Init() {
        // 获取组件
        uiType = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/uiType");
        maxFps = GetNode<OptionButton>("PanelContainer/Scroll/VBoxContainer/maxFps");
        useScreenShader = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/useScreenShader");
        shadow = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/shadow");
        showInfo = GetNode<CheckButton>("PanelContainer/Scroll/VBoxContainer/showInfo");
        exit = GetNode<Button>("PanelContainer/Scroll/VBoxContainer/exit");
        LOD = GetNode<SpinBox>("PanelContainer/Scroll/VBoxContainer/LOD");
        // 设置初始值
        uiType.Selected = (int)ui.uiType;
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
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
            ui.uiType = (UiType)index;
        };
        maxFps.ItemSelected += (index) => {
            Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        };
        useScreenShader.Pressed += () => {
            ui.playerCamera.screenShader.Visible = useScreenShader.ButtonPressed;
        };
        shadow.Pressed += () => {
            light.ShadowEnabled = shadow.ButtonPressed;
        };
        showInfo.Pressed += () => {
            ui.infomation.Visible = showInfo.ButtonPressed;
        };
        LOD.ValueChanged += (value) => {
            ui.GetTree().Root.MeshLodThreshold = (float) value;
        };
        exit.Pressed += () => {
            ui.Exit();
        };
    }
}
