using Godot;
/// <summary>
/// 自动寻路角色管理器
/// </summary>
public class AutoCharacterManager: object {
    public const float speed = 1f;
    /// <summary>
    /// 是否可以边移动边攻击
    /// </summary>
    public bool canAttackWhenMoving = false;
    public GameCharacter character;
    public Player player;
    private bool forceToGo = false;
    public bool ForceToGo {
        get {
            return forceToGo;
        }
        set {
            if (value) {
                state = State.Walk;
            }
            forceToGo = value;
        }
    }
    public GameCharacter target;
    public Vector3 targetPosition;
    public Tool.Void afterAttack;
    private enum State {
        Stop,
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
    public AutoCharacterManager(GameCharacter character, Player player) {
        this.character = character;
        this.player = player;
    }
    private bool IsCloseToTarget() {
        if (target != null && !forceToGo) {
            return character.GlobalPosition.DistanceTo(target.GlobalPosition) <= character.GetAttackRange();
        }
        if (forceToGo) {
            return character.GlobalPosition.DistanceTo(targetPosition) <= 0.5f;
        }
        return false;
    }
    private void SetAgentTarget(float random = 0.5f) {
        character.agent.TargetPosition = targetPosition + Tool.RandomVector3(new Vector3(random, 0, random));
    }
    public void PhysicsProcess(float fDelta) {
        if (player.PlayerState != global::State.move) {
            return;
        }
        switch (state) {
            case State.Stop: {
                break;
            }
            case State.Idle: {
                if (target == null && !forceToGo) {
                    target = GameCharacter.gameCharacters.Find(x => x != character && x.isEnemy != character.isEnemy);
                    if (target == null) { // 没有敌人
                        // 闲逛
                        character.agent.TargetPosition = character.GlobalPosition + Tool.RandomVector3(new Vector3(5, 0, 5));
                        state = State.Walk;
                        break;
                    }
                    target.die += () => {
                        target = null;
                        state = State.Idle;
                    };
                    break;
                }
                if (!forceToGo) {
                    targetPosition = target.GlobalPosition;
                }
                SetAgentTarget();
                if (character.agent.IsTargetReachable()) {
                    state = State.Walk;
                }
                break;
            }
            case State.Walk: {
                // 速度过小，多半是卡住了
                if (character.Velocity.Length() < 0.1f && !IsCloseToTarget()) {
                    stuckCount++;
                    if (stuckCount > 10) {
                        SetAgentTarget();
                        stuckCount = 0;
                    }
                } else {
                    stuckCount = 0;
                }
                character.PlayWalkAnimation();
                if ((IsCloseToTarget() || character.agent.IsNavigationFinished()) && !forceToGo) {
                    state = State.StartAttack;
                    return;
                }
                Move(fDelta);
                break;
            }
            case State.StartAttack: {
                if (!IsCloseToTarget()) {
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
                if (forceToGo) { // 强制移动
                    state = State.Walk;
                    break;
                }
                if (character.Attackable()) { // 可以再次攻击，即攻击缓冲时间结束
                    afterAttack.Invoke();
                    state = State.Idle;
                    break;
                }
                if (canAttackWhenMoving) { // 可以边移动边攻击
                    Move(fDelta);
                }
                break;
            }
        }
        // 重力
        character.Velocity += Player.gravity * fDelta;
        character.MoveAndSlide();
    }
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="fDelta">时间增量</param>
    public void Move(float fDelta) {
        Vector3 target = character.agent.GetNextPathPosition();
        // 转向并添加速度
        Vector3 direction = (target - character.GlobalPosition).Normalized();
        float directionAngle = new Vector2(direction.X, direction.Z).AngleTo(new Vector2(0, -1));
        character.GlobalRotation = new Vector3(character.GlobalRotation.X, Mathf.LerpAngle(character.GlobalRotation.Y, directionAngle, fDelta * 10), character.GlobalRotation.Z);
        character.Velocity = Tool.Vector3To(character.Velocity, direction * speed, fDelta * 10);
    }
    public void Attack() {
        state = State.Attacking;
    }
}
