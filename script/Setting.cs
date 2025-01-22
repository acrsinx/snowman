using Godot;

public partial class Setting : Control {
    public Ui ui;
    public OptionButton uiType;
    public OptionButton maxFps;
    public CheckButton useScreenShader;
    public CheckButton shadow;
    private Light3D light;
    private SpinBox LOD;
    public Button exit;
    public void Init() {
        // 获取组件
        uiType = GetNode<OptionButton>("PanelContainer/VBoxContainer/uiType");
        maxFps = GetNode<OptionButton>("PanelContainer/VBoxContainer/maxFps");
        useScreenShader = GetNode<CheckButton>("PanelContainer/VBoxContainer/useScreenShader");
        shadow = GetNode<CheckButton>("PanelContainer/VBoxContainer/shadow");
        exit = GetNode<Button>("PanelContainer/VBoxContainer/exit");
        LOD = GetNode<SpinBox>("PanelContainer/VBoxContainer/LOD");
        // 设置初始值
        uiType.Selected = (int)ui.uiType;
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        useScreenShader.ButtonPressed = ui.playerCamera.screenShader.Visible;
        light = ui.playerCamera.GetTree().Root.GetChild<Node>(0).GetChild<Node>(0).GetChild<Light3D>(0);
        if (light is null) {
            ui.Log("找不到灯光。");
        }
        shadow.ButtonPressed = light.ShadowEnabled;
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
        LOD.ValueChanged += (value) => {
            ui.GetTree().Root.MeshLodThreshold = (float) value;
        };
        exit.Pressed += () => {
            ui.Exit();
        };
    }
}
