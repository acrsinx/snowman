using Godot;
public partial class Package: Control {
    public Ui ui;
    public Button back;
    public GridContainer stuffTable;
    public TextureRect image;
    public Label label;
    public StuffBox[] stuffs;
    [Export] public PackedScene stuff;
    public override void _Ready() {
        back = GetNode<Button>("PanelContainer/VBoxContainer/leftUp/back");
        stuffTable = GetNode<GridContainer>("PanelContainer/VBoxContainer/HBoxContainer/Scroll/table");
        image = GetNode<TextureRect>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/Image");
        label = GetNode<Label>("PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/Label");
        stuffs = new StuffBox[64];
        Translation.LangageChanged += () => {
            back.Text = Translation.Translate("退");
        };
    }
    public void Init(Ui ui) {
        this.ui = ui;
        back.Pressed += () => {
            ui.Package();
        };
        for (int i = 0; i < 64; i++) {
            stuffs[i] = (StuffBox) stuff.Instantiate();
            stuffTable.AddChild(stuffs[i]);
            stuffs[i].Init(i, new RedFruit(ui), ui, 5);
        }
    }
}
