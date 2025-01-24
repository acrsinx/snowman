using Godot;

public class Plot0_0 : Plot {
    static Plot0_0(){
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
                LoadCharacter("snowdog", "dog", new Vector3(-4, -1.6f, -6));
                PlayAnimation("dog", "talk");
                LookAtCharacter("dog", 0.3f, 1);
                break;
            }
            case 1: {
                PauseAnimation("dog");
                SetCameraPosition();
                break;
            }
        }
    }
}