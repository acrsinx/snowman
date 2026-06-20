using Godot;
public partial class SoundWave: MeshInstance3D {
    private static readonly Material material = GD.Load<Material>("res://material/iron.tres");
    private static readonly Shape3D shape = new SphereShape3D() {
        Radius = 1
    };
    public Area3D area;
    public SoundWave(Player player, Vector3 globalPosition) {
        Mesh = new CylinderMesh() {
            TopRadius = 0.2f,
            BottomRadius = 1f,
            Height = 0.01f,
            RadialSegments = 16,
            Rings = 1,
            Material = material
        };
        player.root.AddChild(this);
        area = new Area3D()
		{
			CollisionLayer = 0b11
		};
        AddChild(area);
		area.AddChild(new CollisionShape3D() {
			Shape = shape
		});
		area.BodyEntered += (body) => {
			HaveCharacter.GetHaveCharacter(body)?.GetCharacter().BeAttack((int) (10 - Scale.X * 5), DamageType.sound, true);
		};
        GlobalPosition = globalPosition;
        Scale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        float fDeltaScale = (float) delta * 5f;
        Scale = new Vector3(Scale.X + fDeltaScale, Scale.Y + fDeltaScale, Scale.Z + fDeltaScale);
        if (Scale.X > 2) {
            QueueFree();
			return;
        }
    }
}
