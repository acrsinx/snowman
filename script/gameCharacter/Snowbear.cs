using System;
using Godot;
public partial class Snowbear: GameCharacter {
    public static PackedScene SnowbearScene = GD.Load<PackedScene>("res://model/snowbear.gltf");
    public const float speed = 1f;
    private enum State {
        Idle,
        Walk,
        StartAttack,
        Attacking
    };
    private State state = State.Idle;
    /// <summary>
    /// 疑似卡住次数
    /// </summary>
    private int stuckCount = 0;
    public Snowbear(Player player): base(SnowbearScene, player, new SphereShape3D() {
        Radius = 0.5f
    }, new Vector3(0, 0.5f, 0), true) {
        health.MaxHealth = 100;
        health.SetFullHealth();
        health.die += () => {
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta) {
        if (player.PlayerState != global::State.move) {
            return;
        }
        float fDelta = (float) delta;
        switch (state) {
            case State.Idle: {
                agent.TargetPosition = player.character.GlobalPosition + Tool.RandomVector3(new Vector3(1, 0, 1));
                if (agent.IsTargetReachable()) {
                    state = State.Walk;
                }
                break;
            }
            case State.Walk: {
                // 速度过小，多半是卡住了
                if (Velocity.Length() < 0.1f) {
                    stuckCount++;
                    if (stuckCount > 10) {
                        agent.TargetPosition = GlobalPosition + Tool.RandomVector3(new Vector3(1, 0, 1));
                        stuckCount = 0;
                    }
                } else {
                    stuckCount = 0;
                }
                Vector3 target = agent.GetNextPathPosition();
                if (!((PlotCharacter) this).IsPlaying()) {
                    ((PlotCharacter) this).PlayAnimation("walk");
                }
                if (agent.IsNavigationFinished()) {
                    state = State.StartAttack;
                    break;
                }
                // 转向并添加速度
                Vector3 direction = (target - GlobalPosition).Normalized();
                float directionAngle = new Vector2(direction.X, direction.Z).AngleTo(new Vector2(0, -1));
                GlobalRotation = new Vector3(GlobalRotation.X, Tool.FloatToAngle(GlobalRotation.Y, directionAngle, fDelta * 10), GlobalRotation.Z);
                Velocity = Tool.Vector3To(Velocity, direction * speed, fDelta * 10);
                break;
            }
            case State.StartAttack: {
                if (!Attackable()) {
                    state = State.Idle;
                }
                if (GlobalPosition.DistanceTo(player.character.GlobalPosition) > 2) {
                    state = State.Idle;
                    break;
                }
                ((PlotCharacter) this).PauseAnimation();
                ((PlotCharacter) this).PlayAnimation("attack");
                Attack();
                break;
            }
            case State.Attacking: {
                if (Attackable()) { // 可以再次攻击，即攻击缓冲时间结束
                    player.character.BeAttack((int)(20.0f / GlobalPosition.DistanceTo(player.character.GlobalPosition)), DamageType.sound, isEnemy);
                    ((PlotCharacter) this).PauseAnimation();
                    state = State.Idle;
                    break;
                }
                break;
            }
        }
        // 重力
        Velocity += Player.gravity * fDelta;
        MoveAndSlide();
    }
    public override int GetAttackWaitTime() {
        return 500;
    }
    public override void CharacterAttack() {
        base.CharacterAttack();
        state = State.Attacking;
    }
}
