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
    public Snowbear(Player player): base(SnowbearScene, player, true) {
        health.MaxHealth = 100;
        health.SetFullHealth();
        health.die += () => {
            QueueFree();
        };
    }
    public override void _PhysicsProcess(double delta) {
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
                Velocity = (target - GlobalPosition).Normalized() * speed;
                break;
            }
            case State.StartAttack: {
                if (!Attackable()) {
                    state = State.Idle;
                }
                player.ui.Log(GlobalPosition.DistanceTo(player.character.GlobalPosition).ToString());
                if (GlobalPosition.DistanceTo(player.character.GlobalPosition) > 2) {
                    player.ui.Log("--------------");
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
                    ((PlotCharacter) this).PauseAnimation();
                    state = State.Idle;
                }
                break;
            }
        }
        // 重力
        Velocity += Player.gravity * fDelta;
        MoveAndSlide();
        player.ui.Log(state.ToString());
    }
    public override int GetAttackWaitTime() {
        return 1000;
    }
    public override void CharacterAttack() {
        base.CharacterAttack();
        state = State.Attacking;
    }
}
