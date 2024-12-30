using Godot;

public partial class Setting : Control {
    public Ui ui;
    public OptionButton uiType;
    public OptionButton maxFps;
    public CheckButton useScreenShader;
    public Button exit;
    public void Init() {
        uiType = GetNode<OptionButton>("PanelContainer/VBoxContainer/uiType");
        maxFps = GetNode<OptionButton>("PanelContainer/VBoxContainer/maxFps");
        exit = GetNode<Button>("PanelContainer/VBoxContainer/exit");
        useScreenShader = GetNode<CheckButton>("PanelContainer/VBoxContainer/useScreenShader");
        uiType.Selected = (int)ui.uiType;
        Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        useScreenShader.ButtonPressed = ui.playerCamera.screenShader.Visible;
        uiType.ItemSelected += (index) => {
            ui.uiType = (UiType)index;
        };
        maxFps.ItemSelected += (index) => {
            Engine.MaxFps = maxFps.GetItemText(maxFps.GetSelectedId()).ToInt();
        };
        useScreenShader.Pressed += () => {
            ui.playerCamera.screenShader.Visible = useScreenShader.ButtonPressed;
        };
        exit.Pressed += () => {
            ui.Exit();
        };
    }
}
