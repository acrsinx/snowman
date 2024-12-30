using Godot.Collections;

public class DoExit : DoSomething {
    public int exitCode;
    public DoExit(Ui ui, Dictionary dict) {
        this.ui = ui;
        exitCode = (int) dict["exit"];
    }
    public override void Do() {
        ui.playerCamera.PlayerState = State.move;
    }
    public override Dictionary ToDictionary() {
        return new() {
            ["exit"] = exitCode
        };
    }
}
