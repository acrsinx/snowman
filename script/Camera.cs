using Godot;
using System;
public partial class Camera: CharacterBody3D, HaveCharacter {
    private float direction = 0.0f;
    /// <summary>
    /// 玩家瞬时速度
    /// </summary>
    public Vector3 thisVelocity = Vector3.Zero;
    /// <summary>
    /// 玩家状态
    /// </summary>
    private State playerState = State.load;
    public State PlayerState {
        set {
            switch (value) {
                case State.move: {
                    canTurn = false;
                    ui.rightUp.Visible = true;
                    ui.captionContainer.Visible = false;
                    ui.speakerLabel.Text = "";
                    ui.captionLabel.Text = "";
                    ui.phoneControl.Visible = ui.uiType != UiType.computer;
                    ui.settingPanel.Visible = false;
                    ui.packagePanel.Visible = false;
                    ui.leftUp.Visible = true;
                    break;
                }
                case State.setting: {
                    ui.phoneControl.Visible = false;
                    ui.settingPanel.Visible = true;
                    ui.packagePanel.Visible = false;
                    ui.rightUp.Visible = true;
                    ui.leftUp.Visible = false;
                    break;
                }
                case State.package: {
                    ui.phoneControl.Visible = false;
                    ui.settingPanel.Visible = false;
                    ui.packagePanel.Visible = true;
                    ui.rightUp.Visible = false;
                    ui.leftUp.Visible = false;
                    break;
                }
                case State.caption: {
                    ui.phoneControl.Visible = false;
                    ui.rightUp.Visible = false;
                    ui.captionContainer.Visible = true;
                    ui.captionLabel.VisibleRatio = 0.0f;
                    ui.leftUp.Visible = false;
                    break;
                }
            }
            playerState = value;
        }
        get {
            return playerState;
        }
    }
    public Panel ControlPanel;
    /// <summary>
    /// 移动时的触摸屏索引
    /// </summary>
    public int moveIndex = -1;
    /// <summary>
    /// 旋转视角时的触摸屏索引
    /// </summary>
    public int rotateIndex = -1;
    public float front, right;
    /// <summary>
    /// 鼠标是否被隐藏，可以操控角色视角
    /// </summary>
    private bool canTurn = false;
    public bool CanTurn {
        set {
            if (ui.uiType == UiType.computer) {
                if (value) { // 设置成可控制视角时，隐藏鼠标
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                } else { // 设置成不可控制视角时，显示鼠标
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                }
            }
            canTurn = value;
        }
        get {
            return canTurn;
        }
    }
    public Vector2 mouseMove;
    public bool jump = false;
    /// <summary>
    /// 跳跃键上次被按下的时间
    /// </summary>
    public long lastJumpTime;
    /// <summary>
    /// 跳跃延迟
    /// </summary>
    public const long jumpDelay = 100;
    public bool isSlow = false;
    [Export] public MeshInstance3D screenShader;
    [Export] public CollisionShape3D player;
    public GameCharacter playerCharacter;
    public CameraManager cameraManager;
    [Export] public PackedScene snowball;
    public static readonly Vector3 gravity = new(0, -30f, 0);
    public static readonly float jumpSpeed = 10.0f;
    public static readonly float mouseSpeed = 0.003f;
    public static readonly float moveSpeed = 0.2f;
    public static readonly float runSpeed = 0.6f;
    public static readonly float maxMouseMove = 0.1f;
    [Export] public Ui ui;
    [Export] public AudioStreamPlayer backgroundMusic;
    public override void _Ready() {
        ui.Log("_Ready");
        Marker3D m = GetChild<Marker3D>(1);
        Camera3D c = m.GetChild<Camera3D>(0);
        cameraManager = new(c, c.GetChild<RayCast3D>(1), this, m);
    }
    public override void _PhysicsProcess(double delta) {
        if (PlayerState != State.move) {
            return;
        }
        float fDelta = (float) delta;
        if (Input.IsActionPressed("alt")) {
            CanTurn = false;
        }
        // 鼠标限速
        if (mouseMove.X > maxMouseMove) {
            mouseMove.X = maxMouseMove;
        } else if (mouseMove.X < -maxMouseMove) {
            mouseMove.X = -maxMouseMove;
        }
        if (mouseMove.Y > maxMouseMove) {
            mouseMove.Y = maxMouseMove;
        } else if (mouseMove.Y < -maxMouseMove) {
            mouseMove.Y = -maxMouseMove;
        }
        if (PlayerState is State.move && CanTurn) {
            cameraManager.UpdateCameraWhenTurning(mouseMove);
        }
        if (IsOnFloor() && PlayerState == State.move) {
            if (ui.uiType == UiType.computer) {
                front = Input.GetAxis("up", "down");
                right = Input.GetAxis("right", "left");
            }
            // 规格化(right, front)
            float length = MathF.Sqrt(right * right + front * front);
            if (length > 0) {
                right /= length;
                front /= length;
            }
            isSlow = Input.IsActionPressed("slow");
            front *= isSlow?moveSpeed:runSpeed;
            right *= isSlow?moveSpeed:runSpeed;
            if (Input.IsActionPressed("alt")) {
                front = 0;
                right = 0;
            }
            float sin = MathF.Sin(cameraManager.cameraMarker.GlobalRotation.Y);
            float cos = MathF.Cos(cameraManager.cameraMarker.GlobalRotation.Y);
            thisVelocity += new Vector3(front * sin - right * cos, 0, front * cos + right * sin);
            // 在跳跃缓冲时间内可以跳跃
            if (lastJumpTime + jumpDelay > ui.totalGameTime) {
                thisVelocity += new Vector3(0, jumpSpeed, 0);
            }
            if (front != 0 || right != 0) { // 移动时
                direction = new Vector2(-right, front).AngleTo(new(0, -1));
                cameraManager.UpdateCameraWhenMoving(fDelta);
            } else {
                if (mouseMove.X != 0 && CanTurn) {
                    direction = FloatTo1(direction, mouseMove.X, fDelta * 10.0f);
                }
            }
            // 在地板上时有阻力
            thisVelocity *= 0.95f;
        } else {
            thisVelocity += gravity * fDelta;
            // 不在地板上时也有阻力，但阻力更小
            thisVelocity *= 0.99f;
        }
        player.Rotation = new Vector3(player.Rotation.X, FloatTo1(player.Rotation.Y, direction, fDelta * 10.0f), player.Rotation.Z);
        // 限速
        float lengthY = MathF.Abs(thisVelocity.Y);
        if (lengthY > 10.0f) {
            thisVelocity.Y *= 10.0f / lengthY;
        }
        float lengthXZ = new Vector2(thisVelocity.X, thisVelocity.Z).Length();
        float maxSpeed = isSlow?1.0f:3.0f;
        if (lengthXZ > maxSpeed) {
            float factor = maxSpeed / lengthXZ;
            thisVelocity.X *= factor;
            thisVelocity.Z *= factor;
        }
        Velocity = thisVelocity;
        // 移动
        MoveAndSlide();
        if (playerState == State.move) {
            cameraManager.UpdateCamera();
        }
        mouseMove = Vector2.Zero;
        jump = false;
        isSlow = false;
        if (!backgroundMusic.Playing) {
            backgroundMusic.Playing = true;
        }
    }
    public override void _Input(InputEvent @event) {
        if (@event is InputEventScreenDrag drag) {
            if (ui.uiType == UiType.phone) {
                if (drag.Index == moveIndex) { // 移动
                    // 此处不用规格化，在移动时会规格化
                    Vector2 moveVector = drag.Position - ControlPanel.GetGlobalRect().Position - ControlPanel.GetGlobalRect().Size / 2;
                    right = -moveVector.X;
                    front = moveVector.Y;
                    return;
                }
                if (drag.Index == rotateIndex) { // 转动视角
                    canTurn = true;
                    mouseMove = -drag.Relative * mouseSpeed;
                    return;
                }
                // 首次滑动时，判断滑动索引
                if (IsInArea(ui.phoneJump, drag.Position)) {
                    return;
                }
                if (IsInArea(ui.phoneAttack, drag.Position)) {
                    return;
                }
                if (IsInArea(ui.phoneSlow, drag.Position)) {
                    return;
                }
                if (IsInArea(ControlPanel, drag.Position)) {
                    moveIndex = drag.Index;
                    return;
                }
                rotateIndex = drag.Index;
                return;
            }
        }
        if (@event is InputEventScreenTouch touch) { // 滑动事件结束时，重置滑动索引
            if (ui.uiType == UiType.phone) {
                if (touch.Pressed) {
                    return;
                }
                if (touch.Index == moveIndex) {
                    right = 0;
                    front = 0;
                    moveIndex = -1;
                } else if (touch.Index == rotateIndex) {
                    rotateIndex = -1;
                }
            }
        }
        if (@event is InputEventMouseMotion motion) {
            if (ui.uiType == UiType.computer) {
                mouseMove = -motion.Relative * mouseSpeed;
                return;
            }
        }
        if (@event is InputEventMouseButton button) {
            if (ui.uiType == UiType.computer) {
                if (button.IsPressed()) {
                    if (PlayerState is State.move) {
                        switch (button.ButtonIndex) {
                            case MouseButton.Left: {
                                if (CanTurn) { // 玩家可控制视角时，攻击
                                    playerCharacter.Attack();
                                    return;
                                }
                                // 玩家不可控制视角时，进入可控制视角的状态
                                CanTurn = true;
                                return;
                            }
                            case MouseButton.WheelUp: {
                                cameraManager.WheelUp();
                                return;
                            }
                            case MouseButton.WheelDown: {
                                cameraManager.WheelDown();
                                return;
                            }
                        }
                    }
                }
            }
        }
        if (@event is InputEventKey key) {
            if (key.Pressed) {
                switch (key.Keycode) {
                    case Key.Space: {
                        Jump();
                        return;
                    }
                    case Key.Key1: {
                        ui.Choose(0);
                        return;
                    }
                    case Key.Key2: {
                        ui.Choose(1);
                        return;
                    }
                    case Key.Key3: {
                        ui.Choose(2);
                        return;
                    }
                }
            }
        }
    }
    public GameCharacter GetCharacter() {
        return playerCharacter;
    }
    public void Jump() {
        if (PlayerState is State.move) {
            lastJumpTime = ui.totalGameTime;
            jump = true;
        }
    }
    public void Shake() {
        cameraManager.cameraShake.StartShake(ui.totalGameTime, 200);
    }
    /// <summary>
    /// 用于将from平滑地移动到to，速度为speed，但不会超过to，返回新的from，from和to都是弧度
    /// </summary>
    /// <param name="from">从</param>
    /// <param name="to">到</param>
    /// <param name="speed">速度（选填）</param>
    /// <returns>新的值</returns>
    public static float FloatTo1(float from, float to, float speed = 0.1f) {
        float PI2 = 2.0f * MathF.PI;
        float newTo = to - Mathf.Floor(to / PI2) * PI2;
        newTo += Mathf.Floor(from / PI2) * PI2;
        if (MathF.Abs(newTo + PI2 - from) < MathF.Abs(newTo - from)) {
            newTo += PI2;
        }
        if (MathF.Abs(newTo - PI2 - from) < MathF.Abs(newTo - from)) {
            newTo -= PI2;
        }
        if (newTo > from) {
            return MathF.Min(from + speed, newTo);
        } else {
            return MathF.Max(from - speed, newTo);
        }
    }
    /// <summary>
    /// 位置是否在指定范围内
    /// </summary>
    /// <param name="area">范围</param>
    /// <param name="position">位置</param>
    /// <returns>是否在</returns>
    public static bool IsInArea(Control area, Vector2 position) {
        return area.GetGlobalRect().HasPoint(position);
    }
    /// <summary>
    /// 位置是否在指定范围内，不能处理旋转，缩放不均匀等情况
    /// </summary>
    /// <param name="area">范围</param>
    /// <param name="position">位置</param>
    /// <returns>是否在</returns>
    public static bool IsInArea(TouchScreenButton area, Vector2 position) {
        CircleShape2D shape = area.Shape as CircleShape2D;
        float Scale = area.Scale.X;
        float radius = shape.Radius * Scale;
        return position.DistanceTo(area.GlobalPosition + 0.5f * Scale * area.TextureNormal.GetSize()) < radius;
    }
}
