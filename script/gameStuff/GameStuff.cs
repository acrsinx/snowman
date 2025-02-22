using Godot;
public partial class GameStuff: object {
    public Ui ui;
    public GameStuff(Ui ui) {
        this.ui = ui;
    }
    public virtual string GetName() {
        return "???";
    }
    public virtual Texture2D GetTexture() {
        return Images.nothing;
    }
    public virtual bool CanUse() {
        return false;
    }
    public virtual void Use() {
    }
}
