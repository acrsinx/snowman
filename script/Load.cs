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
                label.Text = Translation.Translate("加载中");
                break;
            }
            case 1: {
                ui.player.character = new Snowman(ui.player);
                break;
            }
            case 2: {
                // 检查语言包是否存在，不存在则下载
                if (DirAccess.DirExistsAbsolute("user://localization")) {
                    break;
                }
                // 下载语言包
                if (Tool.isDownloading) { // 正在下载
                    return;
                }
                Ui.Log("下载语言包");
                string path = Tool.Download(ui, "https://gitee.com/acrsinx/snowman/releases/download/v0.0/localization.zip", (downloadPath) => {
                    Tool.Unzip(downloadPath);
                    progress++;
                });
                return;
            }
            case 3: {
                // 加载语言列表
                string[] languages = Translation.GetLanguages();
                for (int i = 0; i < languages.Length; i++) {
                    ui.settingPanel.translation.AddItem(languages[i]);
                }
                // 强行刷新，防止语言不变未刷新
                Translation.LangageChanged.Invoke();
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
