using Godot;

public partial class Load : Control {
    public int progress = 0;
    public const int maxProgress = 5;
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
        RefreshProgress();
    }
    private void RefreshProgress() {
        progressBar.Value = (double) progress / maxProgress;
    }
    private void Do() {
        switch (progress) {
            case 0:
                Plot.camera = ui.playerCamera;
                break;
            case 1:
                ui.playerCamera.playerCharacter = new Snowman(ui.playerCamera.player, ui.playerCamera);
                break;
            case 2:
                new Robot(ui.playerCamera).Position = new Vector3(7, 0, 7);
                break;
            case 3:
                new Robot(ui.playerCamera).Position = new Vector3(5, 0, 5);
                break;
            case 4:
                ui.playerCamera.PlayerState = State.move;
                Plot.Check(ui);
                Visible = false;
                break;
        }
        progress++;
        RefreshProgress();
    }
}
