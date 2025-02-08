using Godot;
public partial class Snowbear: GameCharacter {
    public static PackedScene SnowbearScene = GD.Load<PackedScene>("res://model/snowbear.gltf");
    public Snowbear(Player player): base(SnowbearScene, player, true) {
        health.MaxHealth = 100;
        health.SetFullHealth();
        health.die += () => {
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta) {
    }
}
