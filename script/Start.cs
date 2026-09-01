using Godot;
public partial class Start: Node3D {
    private const float MinX = 1.68f;
    private const float MaxX = 1.72f;
    private const float MidX = (MinX + MaxX) / 2.0f;
    private const float Rx = (MaxX - MinX) / 2.0f;
    private const float MinY = 0.69f;
    private const float MaxY = 0.75f;
    private const float MidY = (MinY + MaxY) / 2.0f;
    private const float Ry = (MaxY - MinY) / 2.0f;
    private const float MinZ = -0.78f;
    private const float MaxZ = -0.82f;
    private const float MidZ = (MinZ + MaxZ) / 2.0f;
    private const float Rz = (MaxZ - MinZ) / 2.0f;
    private Camera3D camera;
    public Setting setting;
    public GameInformation gameInformation;
    public Button settingButton;
    public override void _Ready() {
        setting = GetParent<Node>().GetNode<Setting>("Setting");
        camera = GetChild<Camera3D>(1);
        gameInformation = new(setting);
        setting.Init(gameInformation);
        settingButton = GetParent<Node>().GetNode<Button>("SettingButton");
        Translation.LangageChanged += () => {
            if (settingButton != null) {
                settingButton.Text = Translation.Translate("设");
            }
        };
        settingButton.Pressed += () => {
            setting.Visible = true;
            settingButton.Visible = false;
        };
        setting.GetNodeButton("back").Pressed += () => {
            setting.Visible = false;
            if (settingButton != null) {
                settingButton.Visible = true;
            }
        };
        setting.Visible = false;
        gameInformation.LoadInformation(Ui.savePath);
    }
    public override void _Process(double delta) {
        float t = Time.GetTicksMsec() * 0.0001f;
        float ty = Time.GetTicksMsec() * 0.0007f;
        camera.Position = new Vector3(Mathf.Cos(t) * Rx + MidX, Mathf.Sin(ty) * Ry + MidY, -Mathf.Sin(t+0.1f) * Rz + MidZ);
        camera.Rotate(Vector3.Forward, Mathf.Sin(t) * 0.000001f);
    }
    public override void _Input(InputEvent @event) {
        if (!@event.IsAction("next_caption")) {
            return;
        }
        if (setting.Visible) {
            return;
        }
        if (@event is InputEventMouseButton mouseButtonEvent && (!mouseButtonEvent.Pressed)) {
            if (Tool.IsInArea(settingButton, mouseButtonEvent.Position)) {
                return;
            }
            StartGame();
        }
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame() {
        GetTree().Root.AddChild(ResourceLoader.Load<PackedScene>("res://scene/Game.tscn").Instantiate());
        Player player = GetTree().Root.GetChild(1).GetNode<Player>("container/main/player");
        player.Init(setting);
        Translation.LangageChanged -= () => {
            settingButton.Text = Translation.Translate("设");
        };
        Node parent = GetParent<Node>();
        for (int i = 0; i < parent.GetChildCount(); i++) {
            if (parent.GetChild(i) is Setting) {
                continue;
            }
            parent.GetChild(i).QueueFree();
        }
        parent.QueueFree();
        settingButton = null;
    }
}
