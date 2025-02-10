using Godot;
public partial class Robot: GameCharacter {
    public static PackedScene RobotScene = GD.Load<PackedScene>("res://scene/robot.tscn");
    public Node3D weapon;
    public Robot(Player player): base(RobotScene, player, true) {
        health.MaxHealth = 100;
        health.SetFullHealth();
        weapon = GetChild(0).GetChild(0) as Node3D;
        Area3D area = weapon.GetChild(0) as Area3D;
        area.BodyEntered += body => {
            HaveCharacter character = HaveCharacter.GetHaveCharacter(body);
            if (character == null) {
                return;
            };
            character.GetCharacter().BeAttack(10, DamageType.physics, true);
        };
        health.die += () => {
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta) {
        weapon.Position = new Vector3(0, 2, 0);
        weapon.Rotation = new Vector3(1.8f, player.ui.totalGameTime * 0.01f, 0);
        weapon.Translate(new Vector3(0, 0.2f, 0));
        weapon.Scale = new Vector3(1.0f, 5.0f, 1.0f);
    }
}
