using Godot.Collections;

public class DoCaption : DoSomething {
    public int id;
    public DoCaption(Ui ui, Dictionary dict) {
        this.ui = ui;
        id = (int) dict["caption"];
    }
    public override void Do() {
        ui.ShowCaption(id);
    }
    public override Dictionary ToDictionary() {
        return new() {
            ["caption"] = id
        };
    }
}
