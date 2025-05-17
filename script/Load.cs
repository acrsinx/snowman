using Godot;
public partial class Load: Control {
    public int progress = 0;
    public const int maxProgress = 6;
    /// <summary>
    /// 地图
    /// </summary>
    public SubViewport map;
    public Ui ui;
    public Label label;
    public ProgressBar progressBar;
    public override void _Ready() {
        // 获取组件
        label = GetNode<Label>("PanelContainer/VBoxContainer/Label");
        progressBar = GetNode<ProgressBar>("PanelContainer/VBoxContainer/ProgressBar");
    }
    public override void _Process(double delta) {
        if (progress >= maxProgress) {
            return;
        }
        Do();
    }
    public void Init(Ui ui) {
        this.ui = ui;
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
                map = ui.player.root.GetNode<SubViewport>("map");
                map.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
                map.GetChild<Camera3D>(0).Size = Map.mapSizes[0];
                // 加载用户数据
                ui.settingPanel.gameInformation.LoadInformation(Ui.savePath);
                label.Text = Translation.Translate("加载中");
                break;
            }
            case 1: {
                ui.player.character = new Snowman(ui.player);
                break;
            }
            case 2: {
                // 强行刷新，防止语言不变未刷新
                Translation.LangageChanged.Invoke();
                break;
            }
            case 3: {
                ui.settingPanel.gameInformation.Refresh();
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
