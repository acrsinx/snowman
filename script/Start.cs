using Godot;
public partial class Start: Node3D {
    private const float MinX = -1.3f;
    private const float MaxX = -0.3f;
    private const float MidX = (MinX + MaxX) / 2.0f;
    private const float Rx = (MaxX - MinX) / 2.0f;
    public Setting setting;
    public GameInformation gameInformation;
    public Button settingButton;
    public override void _Ready() {
        setting = GetParent<Node>().GetNode<Setting>("Setting");
        gameInformation = new(setting, null, null);
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
        float t = Time.GetTicksMsec() / 5000.0f;
        Position = new Vector3(Mathf.Sin(t) * Rx + MidX, 0.0f, 0.0f);
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
        Player player = GetTree().Root.GetChild(1).GetNode<Player>("player");
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
