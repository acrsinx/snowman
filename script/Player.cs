using System;
using Godot;
/// <summary>
/// 玩家
/// </summary>
public partial class Player: Node3D {
    /// <summary>
    /// 用户界面管理器
    /// </summary>
    public Ui ui;
    /// <summary>
    /// 相机管理器
    /// </summary>
    public CameraManager cameraManager;
    /// <summary>
    /// 屏幕着色器
    /// </summary>
    public MeshInstance3D screenShader;
    /// <summary>
    /// 玩家角色
    /// </summary>
    public GameCharacter character;
    /// <summary>
    /// 玩家欲移动方向
    /// </summary>
    public float front, right;
    /// <summary>
    /// 是否处于慢状态
    /// </summary>
    public bool isSlow = false;
    /// <summary>
    /// 移动速度（慢）
    /// </summary>
    public static readonly float moveSpeed = 0.2f;
    /// <summary>
    /// 移动速度（正常）
    /// </summary>
    public static readonly float runSpeed = 0.6f;
    /// <summary>
    /// 重力加速度 
    /// </summary>
    public static readonly Vector3 gravity = new(0, -30f, 0);
    /// <summary>
    /// 玩家瞬时速度
    /// </summary>
    public Vector3 thisVelocity = Vector3.Zero;
    /// <summary>
    /// 玩家移动方向（弧度制）
    /// </summary>
    private float direction = 0.0f;
    /// <summary>
    /// 鼠标位移
    /// </summary>
    public Vector2 mouseMove;
    /// <summary>
    /// 鼠标移动最大距离
    /// </summary>
    public static readonly float maxMouseMove = 0.1f;
    /// <summary>
    /// 移动时的触摸屏索引
    /// </summary>
    public int moveIndex = -1;
    /// <summary>
    /// 旋转视角时的触摸屏索引
    /// </summary>
    public int rotateIndex = -1;
    /// <summary>
    /// 鼠标速度
    /// </summary>
    public static readonly float mouseSpeed = 0.003f;
    private bool canTurn = false;
    /// <summary>
    /// 鼠标是否被隐藏，可以操控角色视角
    /// </summary>
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
    private State playerState = State.load;
    /// <summary>
    /// 玩家状态
    /// </summary>
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
    /// <summary>
    /// 是否要跳
    /// </summary>
    public bool jump = false;
    /// <summary>
    /// 跳跃键上次被按下的时间
    /// </summary>
    public long lastJumpTime;
    /// <summary>
    /// 跳跃延迟
    /// </summary>
    public const long jumpDelay = 100;
    /// <summary>
    /// 跳跃速度
    /// </summary>
    public static readonly float jumpSpeed = 10.0f;
    public override void _Ready() {
        // 设置用户界面管理器
        ui = GetTree().Root.GetNode<Ui>("Node/ui");
        ui.player = this;
        ui.Log("_Ready");
        // 设置相机管理器
        Marker3D m = GetChild<Marker3D>(0);
        Camera3D c = m.GetChild<Camera3D>(0);
        MeshInstance3D m3d = c.GetChild<MeshInstance3D>(0);
        cameraManager = new(c, c.GetChild<RayCast3D>(1), this, m);
        screenShader = m3d;
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
        if (character.IsOnFloor() && PlayerState == State.move) {
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
                character.character.Rotation = new Vector3(character.character.Rotation.X, Tool.FloatTo(character.character.Rotation.Y, direction, fDelta * 10.0f), character.character.Rotation.Z);
            }
            // 在地板上时有阻力
            thisVelocity *= 0.95f;
        } else {
            thisVelocity += gravity * fDelta;
            // 不在地板上时也有阻力，但阻力更小
            thisVelocity *= 0.99f;
        }
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
        character.Velocity = thisVelocity;
        // 移动
        character.MoveAndSlide();
        if (playerState == State.move) {
            cameraManager.UpdateCamera();
        }
        mouseMove = Vector2.Zero;
        jump = false;
        isSlow = false;
    }
    public override void _Input(InputEvent @event) {
        if (@event is InputEventScreenDrag drag) {
            if (ui.uiType == UiType.phone) {
                if (drag.Index == moveIndex) { // 移动
                    // 此处不用规格化，在移动时会规格化
                    Vector2 moveVector = drag.Position - ui.ControlPanel.GetGlobalRect().Position - ui.ControlPanel.GetGlobalRect().Size * 0.5f;
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
                if (Tool.IsInArea(ui.phoneJump, drag.Position)) {
                    return;
                }
                if (Tool.IsInArea(ui.phoneAttack, drag.Position)) {
                    return;
                }
                if (Tool.IsInArea(ui.phoneSlow, drag.Position)) {
                    return;
                }
                if (Tool.IsInArea(ui.ControlPanel, drag.Position)) {
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
                                    character.Attack();
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
    /// <summary>
    /// 跳
    /// </summary>
    public void Jump() {
        if (PlayerState is State.move) {
            lastJumpTime = ui.totalGameTime;
            jump = true;
        }
    }
}
