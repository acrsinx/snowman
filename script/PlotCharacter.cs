using Godot;

public class PlotCharacter : object {
    public Node3D character;
    public AnimationPlayer animationPlayer;
    public PlotCharacter(Node3D character) {
        this.character = character;
    }
    public static AnimationPlayer GetAnimationPlayer(Node node) {
        if (node is null) {
            return null;
        }
        if (node is AnimationPlayer animationPlayer) {
            return animationPlayer;
        }
        foreach (Node n in node.GetChildren()) {
            AnimationPlayer p = GetAnimationPlayer(n);
            if (p != null) {
                return p;
            }
        }
        Node n1 = node.GetParent();
        for (int i = 0; i < 10; i++) {
            if (n1 is null) {
                return null;
            }
            if (n1 is AnimationPlayer p2) {
                return p2;
            }
            n1 = n1.GetParent();
        }
        return null;
    }
    public void PlayAnimation(string animationName) {
        animationPlayer ??= GetAnimationPlayer(character);
        animationPlayer.Play(animationName);
    }

    public void PauseAnimation() {
        animationPlayer ??= GetAnimationPlayer(character);
        animationPlayer.Pause();
    }
}
