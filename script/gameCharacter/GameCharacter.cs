using Godot;

public partial class GameCharacter : Node3D, HaveCharacter {
    public Node3D character;
    public PhysicsBody3D physicsBody3D;
    public Camera playerCamera;
    public int attackWaitTime = 100;
    public Health health;
    public bool isEnemy = false;
    private long attackStartTime = 0;
    public GameCharacter(PackedScene character, Camera playerCamera, Node parent) {
        this.character = character.Instantiate<Node3D>();
        parent.AddChild(this);
        AddChild(this.character);
        this.playerCamera = playerCamera;
        physicsBody3D = HaveCharacter.GetPhysicsBody3D(this.character);
        health = new(10);
    }
    public override void _Process(double delta) {
        if (GlobalPosition.DistanceTo(Vector3.Zero) > 1000) {
            if (!isEnemy) {
                GlobalPosition = new Vector3(0, 10, 0);
                return;
            }
            QueueFree();
        }
    }
    public void Attack() {
        if (playerCamera.ui.totalGameTime - attackStartTime > attackWaitTime && playerCamera.PlayerState == State.move) {
            CharacterAttack();
        }
    }
    public GameCharacter GetCharacter() {
        return this;
    }
    // 返回值为是否受伤
    public bool BeAttack(int damage, DamageType type, bool isEnemy) {
        if (isEnemy == this.isEnemy) {
            return false;
        }
        health.CurrentHealth -= damage;
        if (this.isEnemy) {
            playerCamera.ui.healthBar.Visible = true;
            playerCamera.ui.healthBar.Value = health.CurrentHealth;
            playerCamera.ui.healthBar.MaxValue = health.MaxHealth;
        }
        if (health.CurrentHealth <= 0) {
            if (this.isEnemy) {
                playerCamera.ui.healthBar.Visible = false;
                QueueFree();
            }
        }
        if (physicsBody3D is RigidBody3D r) {
            r.ApplyCentralImpulse(Vector3.Up * 1000);
        } else if (physicsBody3D is Camera c) {
            c.thisVelocity += Vector3.Up * 1000;
            c.Shake();
        }
        return true;
    }
    public virtual void CharacterAttack() {
        attackStartTime = playerCamera.ui.totalGameTime;
    }
}