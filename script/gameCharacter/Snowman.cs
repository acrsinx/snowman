using System;
using Godot;

public partial class Snowman : GameCharacter {
    public static PackedScene SnowmanScene = ResourceLoader.Load<PackedScene>("res://model/snowman.gltf");
    public ObjectPool snowballPool = new(10);
    public Snowman(Node parent, Camera playerCamera) : base(SnowmanScene, playerCamera, parent, false) {
        Position += new Vector3(0, -1, 0);
        Rotate(Vector3.Up, 0.5f*MathF.PI);
        health.MaxHealth = 1000;
        health.SetFullHealth();
    }
    public override void CharacterAttack() {
        base.CharacterAttack();
        Node3D snowball;
        RigidBody3D rigidBody;
        snowball = snowballPool.Add(playerCamera.snowball);
        if (snowball == null) {
            return;
        }
        playerCamera.GetTree().Root.AddChild(snowball);
        rigidBody = snowball.GetChild<RigidBody3D>(0);
        CollisionShape3D shape = rigidBody.GetChild<CollisionShape3D>(1);
        shape.Scale = new Vector3(0.1f, 0.1f, 0.1f);
        rigidBody.ContactMonitor = true;
        rigidBody.MaxContactsReported = 1;
        // 设置雪球位置
        snowball.GlobalPosition = playerCamera.player.GlobalPosition;
        snowball.GlobalRotation = new(playerCamera.cameraMarker.Rotation.X, playerCamera.player.GlobalRotation.Y, 0);
        snowball.Translate(new Vector3(0, 0, -0.5f));
        // 设置速度
        Vector3 direction = new Vector3(0, 0, -1).Rotated(new(0, 1, 0), snowball.GlobalRotation.Y).Rotated(new(1, 0, 0), snowball.GlobalRotation.X) + new Vector3(0, 0.5f, 0);
        rigidBody.SetAxisVelocity(playerCamera.Velocity);
        Vector3 impuse = direction.Normalized()*10;
        rigidBody.ApplyImpulse(impuse, new Vector3(0, 0, 0));
        // I = mv => v = I/m
        playerCamera.thisVelocity -= impuse * 0.1f;
    }
    public override void _PhysicsProcess(double delta) {
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
    }
}
