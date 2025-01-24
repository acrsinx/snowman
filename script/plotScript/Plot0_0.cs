public class Plot0_0 : Plot {
    static Plot0_0() {
        paths = new string[] {
            "res://plotJson/plot0/plot0_0.json",
            "res://plotJson/plot0/plot0_1.json",
            "res://plotJson/plot0/plot0_2.json",
        };
    }
    public override void Animate(int id, Ui ui, bool isEnd, int code) {
        switch (id) {
            case 0: {
                if (isEnd) {
                    return;
                }
                ParseScriptLine("LoadCharacter(snowdog, dog, (-4, -1.6, -6))");
                ParseScriptLine("PlayAnimation(dog, talk)");
                ParseScriptLine("LookAtCharacter(dog, 0.3, 1)");
                break;
            }
            case 1: {
                ParseScriptLine("PauseAnimation(dog)");
                ParseScriptLine("SetCameraPosition()");
                break;
            }
        }
    }
}