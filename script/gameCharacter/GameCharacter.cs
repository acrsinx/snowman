using System;
using Godot;
public partial class GameCharacter: CharacterBody3D, HaveCharacter, PlotCharacter {
    public Node3D character;
    public PhysicsBody3D physicsBody3D;
    public CollisionShape3D collisionShape3D;
    private AnimationPlayer animationPlayer;
    /// <summary>
    /// 寻路节点
    /// </summary>
    public NavigationAgent3D agent;
    public AnimationPlayer AnimationPlayer {
        get => animationPlayer;
        set => animationPlayer = value;
    }
    /// <summary>
    /// 小地图标记
    /// </summary>
    public Sprite2D mapFlag;
    public Player player;
    public Health health;
    public bool isEnemy = false;
    public bool isPlayer = false;
    private long attackStartTime = 0;
    public GameCharacter(PackedScene character, Player player, Shape3D shape3D, Vector3 offsetPosition, bool isEnemy, bool isPlayer = false) {
        this.character = character.Instantiate<Node3D>();
        this.isEnemy = isEnemy;
        this.isPlayer = isPlayer;
        if (isPlayer) {
            player.AddChild(this);
        } else {
            player.GetTree().Root.AddChild(this);
        }
        AddChild(this.character);
        // 添加寻路节点
        agent = new NavigationAgent3D();
        # if TOOLS
        agent.DebugEnabled = true;
        # endif
        AddChild(agent);
        collisionShape3D = new CollisionShape3D() {
            Shape = shape3D,
            Position = offsetPosition
        };
        AddChild(collisionShape3D);
        // 添加小地图标记
        mapFlag = new Sprite2D {
            Scale = new Vector2(0.1f, 0.1f)
        };
        if (isEnemy) {
            mapFlag.Texture = ResourceLoader.Load<Texture2D>("res://image/redFruit.png");
        } else {
            mapFlag.Texture = ResourceLoader.Load<Texture2D>("res://image/playerFlag.svg");
        }
        player.ui.panel.AddChild(mapFlag);
        this.player = player;
        physicsBody3D = HaveCharacter.GetPhysicsBody3D(this.character);
        health = new(10);
    }
    public override void _Process(double delta) {
        if (GlobalPosition.DistanceTo(Vector3.Zero) > 1000) {
            if (!isEnemy) {
                GlobalPosition = new Vector3(0, 10, 0);
                return;
            }
            Die();
        }
        if (player.PlayerState == State.move) {
            // 刷新小地图标记
            mapFlag.Position = Map.GlobalPositionToMapPosition(player, character.GlobalPosition);
            mapFlag.GlobalRotation = -MathF.PI * 0.5f - character.GlobalRotation.Y;
        }
    }
    public bool Attackable() {
        return player.ui.totalGameTime - attackStartTime > GetAttackWaitTime() && player.PlayerState == State.move;
    }
    public bool Attack() {
        if (!Attackable()) {
            return false;
        }
        CharacterAttack();
        return true;
    }
    public GameCharacter GetCharacter() {
        return this;
    }
    public PlotCharacter GetPlotCharacter() {
        return this;
    }
    /// <summary>
    /// 受到攻击
    /// </summary>
    /// <param name="damage">基础伤害值</param>
    /// <param name="type">伤害类型</param>
    /// <param name="isEnemy">攻击者是否是敌人</param>
    /// <returns>是否受伤</returns>
    public bool BeAttack(int damage, DamageType type, bool isEnemy) {
        if (isEnemy == this.isEnemy) {
            return false;
        }
        health.CurrentHealth -= damage;
        if (this.isEnemy) {
            player.ui.healthBar.Visible = true;
            player.ui.healthBar.Value = health.CurrentHealth;
            player.ui.healthBar.MaxValue = health.MaxHealth;
        }
        if (health.CurrentHealth <= 0) {
            if (this.isEnemy) {
                player.ui.healthBar.Visible = false;
                Die();
            }
        }
        if (physicsBody3D is RigidBody3D r) {
            r.ApplyCentralImpulse(Vector3.Up * 1000);
        }
        if (isPlayer) {
            player.cameraManager.cameraShake.StartShake(player.ui.totalGameTime, 500);
        }
        return true;
    }
    public virtual int GetAttackWaitTime() {
        return 100;
    }
    public virtual void CharacterAttack() {
        attackStartTime = player.ui.totalGameTime;
    }
    public virtual void PlayWalkAnimation() {
    }
    public void Die() {
        mapFlag.QueueFree();
        QueueFree();
    }
    public Node3D GetCharacterNode() {
        return character;
    }
}
