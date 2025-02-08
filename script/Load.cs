using Godot;
public partial class Load: Control {
    public int progress = 0;
    public const int maxProgress = 6;
    /// <summary>
    /// 地图
    /// </summary>
    public SubViewport map;
    public Ui ui;
    public ProgressBar progressBar;
    public override void _Ready() {
        // 获取组件
        progressBar = GetNode<ProgressBar>("PanelContainer/VBoxContainer/ProgressBar");
    }
    public override void _Process(double delta) {
        if (progress >= maxProgress) {
            return;
        }
        Do();
    }
    public void Init() {
        Visible = true;
        RefreshProgress();
    }
    private void RefreshProgress() {
        progressBar.Value = progress * 100.0D / maxProgress;
    }
    private void Do() {
        switch (progress) {
            case 0: {
                Plot.player = ui.player;
                map = GetTree().Root.GetNode<SubViewport>("Node/map");
                map.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
                map.GetChild<Camera3D>(0).Size = Map.mapSizes[0];
                // 加载用户数据
                ui.gameInformation.LoadInformation(Ui.savePath);
                break;
            }
            case 1: {
                ui.player.character = new Snowman(ui.player);
                break;
            }
            case 2: {
                new Robot(ui.player).Position = new Vector3(7, 0, 7);
                break;
            }
            case 3: {
                new Robot(ui.player).Position = new Vector3(5, 0, 5);
                break;
            }
            case 4: {
                map.GetTexture().GetImage().SavePng("user://map.png");
                map.QueueFree();
                break;
            }
            case 5: {
                ui.player.PlayerState = State.move;
                Plot.Check(ui);
                Visible = false;
                ui.map.Texture = ImageTexture.CreateFromImage(Image.LoadFromFile("user://map.png"));
                QueueFree();
                break;
            }
        }
        progress++;
        RefreshProgress();
    }
}
