using Godot;
public partial class Snowdog: GameCharacter {
    public static readonly PackedScene SnowdogScene = GD.Load<PackedScene>("res://model/snowdog.glb");
    public Snowdog(Player player): base(SnowdogScene, player, new SphereShape3D() {
        Radius = 0.25f
    }, new Vector3(0, 0.5f, 0), false) {
        PlotCharacter.AddAnimationPlayer(this, "fourFeet");
        health.MaxHealth = 100;
        health.SetFullHealth();
        health.die += () => {
            QueueFree();
        };
    }
}
