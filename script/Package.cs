using Godot;

public partial class Package : Control {
    public Ui ui;
    public Button back;
    public GridContainer stuffTable;
    public StuffBox[] stuffs;
    [Export] public PackedScene stuff;
    public override void _Ready() {
        back = GetNode<Button>("PanelContainer/VBoxContainer/leftUp/back");
        stuffTable = GetNode<GridContainer>("PanelContainer/VBoxContainer/HBoxContainer/Scroll/table");
        stuffs = new StuffBox[64];
    }
    public void Init() {
        back.Pressed += () => {
            ui.Package();
        };
        for (int i = 0; i < 64; i++) {
            stuffTable.AddChild(stuff.Instantiate());
            stuffs[i] = stuffTable.GetChild<StuffBox>(i);
            stuffs[i].Init(i, new RedFruit(ui), ui, 5);
        }
    }
}
