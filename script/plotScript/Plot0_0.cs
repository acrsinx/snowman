using Godot;

public class Plot0_0 : Plot {
    Node3D dog;
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
                PackedScene snowdog = ResourceLoader.Load<PackedScene>("res://model/snowdog.gltf");
                dog = snowdog.Instantiate<Node3D>();
                dog.Position = new Vector3(-4, -1, -6);
                ui.playerCamera.GetParent().AddChild(dog);
                AnimationPlayer animation = dog.GetChild<AnimationPlayer>(1);
                animation.Play("talk");
                ui.playerCamera.cameraManager.MoveCamera(new Vector3(-4, 0, -6));
                break;
            }
            case 1: {
                AnimationPlayer animation = dog.GetChild<AnimationPlayer>(1);
                animation.Pause();
                ui.playerCamera.cameraManager.SetCameraPosition();
                break;
            }
        }
    }
}