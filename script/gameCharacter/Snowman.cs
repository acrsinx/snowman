using Godot;
public partial class Snowman: GameCharacter {
    public static readonly PackedScene SnowmanScene = ResourceLoader.Load<PackedScene>("res://model/snowman.gltf");
    public static readonly PackedScene SnowballScene = ResourceLoader.Load<PackedScene>("res://model/snowball.gltf");
    public static readonly Vector3 snowballOffset = new(0, 1.0f, 0);
    public ObjectPool snowballPool = new(10);
    public Snowman(Player player, bool isPlayer = true): base(SnowmanScene, player, new CylinderShape3D() {
        Radius = 0.25f,
        Height = 0.9f
    }, new Vector3(0, 0.5f, 0), false, isPlayer) {
        Position = new Vector3(0, 0.1f, 0);
        if (isPlayer) {
            player.cameraManager.cameraMarker.Reparent(this, false);
            player.cameraManager.SetCameraPosition();
        } else {
            auto = new AutoCharacterManager(this, player) {
                canAttackWhenMoving = true
            };
        }
        health.MaxHealth = 1000;
        health.SetFullHealth();
    }
    public override float GetAttackRange() {
        return 7;
    }
    public override void CharacterAttack() {
        base.CharacterAttack();
        Node3D snowball;
        RigidBody3D rigidBody;
        snowball = snowballPool.Add(SnowballScene);
        if (snowball == null) {
            return;
        }
        player.GetTree().Root.AddChild(snowball);
        rigidBody = snowball.GetChild<RigidBody3D>(0);
        CollisionShape3D shape = rigidBody.GetChild<CollisionShape3D>(1);
        shape.Scale = new Vector3(0.1f, 0.1f, 0.1f);
        rigidBody.ContactMonitor = true;
        rigidBody.MaxContactsReported = 1;
        // 设置雪球位置
        snowball.GlobalPosition = GlobalPosition + snowballOffset;
        snowball.GlobalRotation = new(isPlayer?player.cameraManager.cameraMarker.Rotation.X:Tool.RandomFloat(0.5f, 0.5001f), character.GlobalRotation.Y, 0);
        snowball.Translate(new Vector3(0, 0, -0.6f));
        // 设置速度
        Vector3 direction = new Vector3(0, 0, -1).Rotated(new(0, 1, 0), snowball.GlobalRotation.Y).Rotated(new(1, 0, 0), snowball.GlobalRotation.X) + new Vector3(0, 0.5f, 0);
        rigidBody.SetAxisVelocity(Velocity);
        Vector3 impuse = direction.Normalized() * 10;
        rigidBody.ApplyImpulse(impuse, new Vector3(0, 0, 0));
        // I = mv => v = I/m
        Velocity -= impuse * 0.1f;
    }
    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        for (int i = 0; i < snowballPool.Count; i++) {
            if (!snowballPool.haveUsed[i]) {
                continue;
            }
            Node3D snowball = snowballPool.Get(i);
            RigidBody3D body = snowball.GetChild<RigidBody3D>(0);
            if (body.GetContactCount() > 0) {
                HaveCharacter target = HaveCharacter.GetHaveCharacter(body.GetCollidingBodies()[0]);
                target?.GetCharacter().BeAttack(10, DamageType.snow, false);
                snowballPool.Remove(i);
                snowball.QueueFree();
                continue;
            }
        }
        if (player.PlayerState != State.move) {
            return;
        }
        if (auto == null) {
            return;
        }
    }
}
