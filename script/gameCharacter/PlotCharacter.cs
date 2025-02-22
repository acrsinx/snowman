using Godot;
public interface PlotCharacter {
    public Node3D GetCharacterNode();
    public AnimationPlayer AnimationPlayer {
        set;
        get;
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
        if (!CheckAnimationPlayer()) {
            return;
        }
        AnimationPlayer.Play(animationName);
    }
    public void PauseAnimation() {
        if (!CheckAnimationPlayer()) {
            return;
        }
        AnimationPlayer.Pause();
    }
    public bool IsPlaying() {
        if (!CheckAnimationPlayer()) {
            return false;
        }
        return AnimationPlayer.IsPlaying();
    }
    /// <summary>
    /// 获取当前播放的动画名称。如果动画播放器不存在，返回空字符串。如果当前没有播放动画，返回空字符串。
    /// </summary>
    /// <returns>返回动画名</returns>
    public string GetAnimationName() {
        if (!CheckAnimationPlayer()) {
            return "";
        }
        if (AnimationPlayer.CurrentAnimation == null) {
            return "";
        }
        return AnimationPlayer.CurrentAnimation;
    }
    private bool CheckAnimationPlayer() {
        AnimationPlayer ??= GetAnimationPlayer(GetCharacterNode());
        if (AnimationPlayer == null) {
            return false;
        }
        return true;
    }
}
