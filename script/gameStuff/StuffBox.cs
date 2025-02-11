using Godot;
public partial class StuffBox: BoxContainer {
    public int id = -1;
    public GameStuff stuff;
    public Ui ui;
    private int num = 0;
    public int Num {
        get {
            return num;
        }
        set {
            if (value < 0) {
                num = 0;
                Update();
                return;
            }
            num = value;
            Update();
        }
    }
    public void Init(int id, GameStuff stuff, Ui ui, int num) {
        this.id = id;
        this.stuff = stuff;
        this.ui = ui;
        Num = num;
        Button use = GetChild<Button>(2);
        use.MouseEntered += () => {
            Focus();
        };
        use.FocusEntered += () => {
            Focus();
        };
        use.Pressed += () => {
            if (Num <= 0) {
                return;
            }
            if (!ui.CanUse(this.stuff)) {
                return;
            }
            ui.Use(this.stuff);
            Num--;
        };
    }
    /// <summary>
    /// 鼠标悬停或被聚焦
    /// </summary>
    public void Focus() {
        ui.packagePanel.image.Texture = stuff.GetTexture();
        ui.packagePanel.label.Text = stuff.GetName();
    }
    public void Update() {
        if (Num < 0) {
            ui.Log("为什么会有负数的物品？");
            return;
        }
        if (Num == 0) {
            if (stuff is not Nothing) {
                stuff = new Nothing(ui);
            }
            GetChild<TextureRect>(0).Texture = stuff.GetTexture();
            GetChild<Label>(1).Text = "";
            return;
        }
        GetChild<TextureRect>(0).Texture = stuff.GetTexture();
        GetChild<Label>(1).Text = stuff.GetName() + ": " + num.ToString();
    }
}
