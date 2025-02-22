using Godot;
/// <summary>
/// 自动寻路角色管理器
/// </summary>
public class AutoCharacterManager : object {
    public const float speed = 1f;
	public GameCharacter character;
	public Player player;
	public GameCharacter target;
	public delegate void voidDelegate();
	public voidDelegate afterAttack;
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
	public AutoCharacterManager(GameCharacter character, Player player, GameCharacter target) {
	    this.character = character;
        this.player = player;
        this.target = target;
	}
	public void PhysicsProcess(float fDelta) {
        if (player.PlayerState != global::State.move) {
            return;
        }
        switch (state) {
            case State.Idle: {
                character.agent.TargetPosition = target.GlobalPosition + Tool.RandomVector3(new Vector3(1, 0, 1));
                if (character.agent.IsTargetReachable()) {
                    state = State.Walk;
                }
                break;
            }
            case State.Walk: {
                // 速度过小，多半是卡住了
                if (character.Velocity.Length() < 0.1f) {
                    stuckCount++;
                    if (stuckCount > 10) {
                        character.agent.TargetPosition = character.GlobalPosition + Tool.RandomVector3(new Vector3(1, 0, 1));
                        stuckCount = 0;
                    }
                } else {
                    stuckCount = 0;
                }
                Vector3 target = character.agent.GetNextPathPosition();
				character.PlayWalkAnimation();
                if (character.agent.IsNavigationFinished()) {
                    state = State.StartAttack;
                    break;
                }
                // 转向并添加速度
                Vector3 direction = (target - character.GlobalPosition).Normalized();
                float directionAngle = new Vector2(direction.X, direction.Z).AngleTo(new Vector2(0, -1));
                character.GlobalRotation = new Vector3(character.GlobalRotation.X, Tool.FloatToAngle(character.GlobalRotation.Y, directionAngle, fDelta * 10), character.GlobalRotation.Z);
                character.Velocity = Tool.Vector3To(character.Velocity, direction * speed, fDelta * 10);
                break;
            }
            case State.StartAttack: {
                if (character.GlobalPosition.DistanceTo(player.character.GlobalPosition) > 2) {
                    state = State.Idle;
                    break;
                }
                if (!character.Attack()) {
					state = State.Idle;
                    break;
				}
                break;
            }
            case State.Attacking: {
                if (character.Attackable()) { // 可以再次攻击，即攻击缓冲时间结束
					afterAttack.Invoke();
                    state = State.Idle;
                    break;
                }
                break;
            }
        }
        // 重力
        character.Velocity += Player.gravity * fDelta;
        character.MoveAndSlide();
	}
	public void Attack() {
		state = State.Attacking;
	}
}