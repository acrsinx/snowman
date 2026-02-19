using Godot;
public partial class Load: Control {
    /// <summary>
    /// 进度（-1: 停止）
    /// </summary>
    public int progress = 0;
    public const int maxProgress = 12;
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
        if (progress >= maxProgress || progress < 0) {
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
                ui.player.character = new Snowman(ui.player);
                if (!DirAccess.DirExistsAbsolute("user://maps")) {
                    DirAccess.MakeDirAbsolute("user://maps");
                }
                break;
            }
            case 1: {
                // 加载地图摄像机
                map = ui.player.root.GetNode<SubViewport>("map");
                map.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;
                map.GetChild<Camera3D>(0).Size = Map.mapSizes[0];
                break;
            }
            case 2: {
                // 加载用户数据
                ui.settingPanel.gameInformation.LoadInformation(Ui.savePath);
                label.Text = Translation.Translate("加载中");
                break;
            }
            case 3: {
                // 强行刷新，防止语言不变未刷新
                Translation.LangageChanged.Invoke();
                break;
            }
            case 4: {
                ui.SetScene("battlefield");
                break;
            }
            case 5: {
                map.GetTexture().GetImage().SavePng("user://maps/battlefield_map.png");
                break;
            }
            case 6: {
                ui.SetScene("base");
                break;
            }
            case 7: {
                map.GetTexture().GetImage().SavePng("user://maps/base_map.png");
                break;
            }
            case 8: {
                map.QueueFree();
                break;
            }
            case 9: {
                ui.SetScene("battlefield");
                break;
            }
            case 10: {
                ui.settingPanel.gameInformation.Refresh();
                break;
            }
            case 11: {
                ui.player.PlayerState = State.move;
                Plot.Check(ui);
                Visible = false;
                QueueFree();
                break;
            }
        }
        progress++;
        RefreshProgress();
    }
}
