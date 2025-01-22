using Godot;

public partial class StuffBox : BoxContainer {
    public int id = -1;
    public GameStuff stuff;
    public Ui ui;
    private int num = 0;
    public int Num {
        get { return num; }
        set {
            if (value < 0) num = 0;
            num = value;
        }
    }
    public void Init(int id, GameStuff stuff, Ui ui, int num) {
        this.id = id;
        this.stuff = stuff;
        this.ui = ui;
        Num = num;
        Update();
        GetChild<Button>(2).Pressed += () => {
            if (Num <= 0) {
                return;
            }
            if (!ui.CanUse(stuff)) {
                return;
            }
            ui.Use(stuff);
            Num--;
            Update();
        };
    }

    public void Update() {
        GetChild<TextureRect>(0).Texture = stuff.GetTexture();
        GetChild<Label>(1).Text = stuff.GetName() + ": " + num.ToString();
    }
}