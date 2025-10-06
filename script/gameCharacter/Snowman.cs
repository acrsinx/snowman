using Godot;
public partial class Snowman: GameCharacter {
    public static readonly PackedScene SnowmanScene = ResourceLoader.Load<PackedScene>("res://model/snowman.gltf");
    public static readonly Mesh SnowballMesh = ResourceLoader.Load<Mesh>("res://model/snowball.tres");
    public static readonly Vector3 snowballOffset = new(0, 1.0f, 0);
    public static readonly Vector3 gravity = new(0, -9.8f, 0);
    public static RayCast3D checkCast;
    public static ObjectPool snowballPool = new(32, SnowballMesh);
    public Snowman(Player player, bool isPlayer = true): base(SnowmanScene, player, new CylinderShape3D() {
        Radius = 0.25f,
        Height = 0.9f
    }, new Vector3(0, 0.5f, 0), false, isPlayer) {
        if (checkCast == null) { // 第一次初始化雪人
            checkCast = new() {
                CollisionMask = 0b10
            };
            player.root.AddChild(checkCast);
            player.root.AddChild(snowballPool.instances);
        }
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
        int id = snowballPool.Add(this);
        if (id == -1) {
            return;
        }
        // 设置雪球位置
        Transform3D snowballTransform = Transform3D.Identity;
        snowballTransform = snowballTransform.Scaled(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 SnowballGlobalPosition = GlobalPosition + snowballOffset;
        snowballTransform = snowballTransform.Translated(SnowballGlobalPosition);
        snowballTransform = snowballTransform.TranslatedLocal(new Vector3(0, -0.6f, 0));
        snowballPool.instances.Multimesh.SetInstanceTransform(id, snowballTransform);
        // 设置速度
        float rx = isPlayer?player.cameraManager.cameraMarker.Rotation.X:Tool.RandomFloat(0.5f, 0.5001f);
        float ry = character.GlobalRotation.Y;
        Vector3 direction = new Vector3(0, 0, -1).Rotated(new(0, 1, 0), ry).Rotated(new(1, 0, 0), rx) + new Vector3(0, 0.5f, 0);
        snowballPool.Velocities[id] = Velocity + direction * 10;
        Vector3 impuse = direction.Normalized() * 10;
        // I = mv => v = I/m
        Velocity -= impuse * 0.1f;
    }
    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (isPlayer) {
            PhysicsProcess((float) delta);
        }
        if (player.PlayerState != State.move) {
            return;
        }
        if (auto == null) {
            return;
        }
    }
    public static void PhysicsProcess(float fDelta) {
        for (int i = 0; i < snowballPool.Count; i++) {
            if (!snowballPool.haveUsed[i]) {
                continue;
            }
            // 设置雪球位置
            Transform3D snowballTransform = snowballPool.instances.Multimesh.GetInstanceTransform(i);
            Vector3 origin = snowballTransform.Origin;
            // v += g * t
            snowballPool.Velocities[i] += gravity * fDelta;
            // x += v * t
            Vector3 deltaPosition = snowballPool.Velocities[i] * fDelta;
            snowballTransform = snowballTransform.Translated(deltaPosition);
            // 检测碰撞
            checkCast.GlobalPosition = origin;
            checkCast.TargetPosition = deltaPosition;
            checkCast.ForceRaycastUpdate();
            if (checkCast.IsColliding()) {
                HaveCharacter target = HaveCharacter.GetHaveCharacter((Node) checkCast.GetCollider());
                if (target?.GetCharacter() == snowballPool.owners[i]) {
                    continue;
                }
                target?.GetCharacter().BeAttack(10, DamageType.snow, false);
                snowballPool.instances.Multimesh.SetInstanceTransform(i, Tool.ZeroTransform3D);
                snowballPool.Remove(i);
                continue;
            }
            // 设置雪球位置
            snowballPool.instances.Multimesh.SetInstanceTransform(i, snowballTransform);
        }
    }
}
