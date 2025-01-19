using Godot.Collections;

public class DoCaption : DoSomething {
    public int id;
    public DoCaption(Ui ui, Dictionary dict) {
        this.ui = ui;
        id = (int) dict["caption"];
    }
    public override void Do(Plot plot, int captionIndex) {
        ui.ShowCaption(id);
        plot.Animate(captionIndex, ui, true, 0);
    }
    public override Dictionary ToDictionary() {
        return new() {
            ["caption"] = id
        };
    }
}
