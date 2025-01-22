using Godot.Collections;

public class DoExit : DoSomething {
    public int exitCode;
    public DoExit(Ui ui, Dictionary dict) {
        this.ui = ui;
        exitCode = (int) dict["exit"];
    }
    public override void Do(Plot plot, int captionIndex) {
        ui.playerCamera.PlayerState = State.move;
        plot.Animate(captionIndex, ui, true, exitCode);
    }
    public override Dictionary ToDictionary() {
        return new() {
            ["exit"] = exitCode
        };
    }
}
