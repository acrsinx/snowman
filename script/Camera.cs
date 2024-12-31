using Godot;
using System;

public partial class Camera : CharacterBody3D, HaveCharacter {
    // 相机标志的原位置
    public static readonly Vector3 CameraMarkerOrigin = new(0, 2.633f, 0);
    private float direction = 0.0f;
    public Vector3 thisVelocity = Vector3.Zero;
    // 玩家状态
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
                    break;
                }
                case State.setting: {
                    ui.phoneControl.Visible = false;
                    ui.settingPanel.Visible = true;
                    ui.packagePanel.Visible = false;
                    ui.rightUp.Visible = true;
                    break;
                }
                case State.package: {
                    ui.phoneControl.Visible = false;
                    ui.settingPanel.Visible = false;
                    ui.packagePanel.Visible = true;
                    ui.rightUp.Visible = false;
                    break;
                }
                case State.caption: {
                    ui.phoneControl.Visible = false;
                    ui.rightUp.Visible = false;
                    ui.captionContainer.Visible = true;
                    ui.captionLabel.VisibleRatio = 0.0f;
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
    public float front, right;
    // 鼠标是否被隐藏，可以操控角色视角
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
    public bool isSlow = false;
    public Shake cameraShake = new();
    [Export] public Marker3D cameraMarker;
    [Export] public MeshInstance3D screenShader;
    [Export] public CollisionShape3D player;
    public GameCharacter playerCharacter;
    public CameraManager cameraManager;
    [Export] public PackedScene snowball;
    [Export] public Vector3 gravity = new(0, -30f, 0);
    [Export] public float jumpSpeed = 15.0f;
    [Export] public float mouseSpeed = 0.003f;
    [Export] public float moveSpeed = 0.2f;
    [Export] public float runSpeed = 2.0f;
    [Export] public float maxMouseMove = 0.1f;
    [Export] public Ui ui;
    [Export] public AudioStreamPlayer backgroundMusic;
    public override void _Ready() {
        ui.Log("_Ready");
        Camera3D c = GetChild<Node3D>(1).GetChild<Camera3D>(0);
        cameraManager = new(c, c.GetChild<RayCast3D>(1));
    }
    public override void _PhysicsProcess(double delta) {
        if (PlayerState == State.load) {
            playerCharacter = new Snowman(player, this);
            new Robot(this).Position = new Vector3(15, -2.0f, 15);
            new Robot(this).Position = new Vector3(21, -2.0f, 12);
            PlayerState = State.move;
            Plot.Check(ui);
            return;
        }
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
        if (PlayerState is State.move&&CanTurn) {
            // 处理cameraMarker.Rotation
            cameraMarker.Rotation = new Vector3(cameraMarker.Rotation.X+mouseMove.Y, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
            // 处理this.Rotation
            Rotation = new Vector3(Rotation.X, Rotation.Y+mouseMove.X, Rotation.Z);
            // 限制视角
            if (-1.2f>cameraMarker.Rotation.X) {
                cameraMarker.Rotation = new Vector3(-1.2f, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
            } else if (0.5f<cameraMarker.Rotation.X) {
                cameraMarker.Rotation = new Vector3(0.5f, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
            }
            if (ui.uiType == UiType.computer) {
                // 鼠标归中
                Input.WarpMouse(0.5f * GetViewport().GetVisibleRect().Size);
            }
        }
        if (IsOnFloor()&&PlayerState==State.move) {
            float front, right;
            if (ui.uiType == UiType.computer) {
                front = Input.GetAxis("ui_up", "ui_down");
                right = Input.GetAxis("ui_right", "ui_left");
            } else {
                front = this.front;
                right = this.right;
            }
            isSlow = ui.phoneSlow.ButtonPressed || Input.IsMouseButtonPressed(MouseButton.Right);
            front *= isSlow?moveSpeed:runSpeed;
            right *= isSlow?moveSpeed:runSpeed;
            if (Input.IsActionPressed("alt")) {
                front = 0;
                right = 0;
            }
            float sin = MathF.Sin(cameraMarker.GlobalRotation.Y);
            float cos = MathF.Cos(cameraMarker.GlobalRotation.Y);
            thisVelocity += new Vector3(front*sin-right*cos, 0, front*cos+right*sin);
            thisVelocity += new Vector3(0, jump?jumpSpeed:0.0f, 0);
            if (front!=0||right!=0) {
                direction = new Vector2(-right, front).AngleTo(new(0, -1));
            } else {
                if (mouseMove.X != 0&&CanTurn) {
                    direction = FloatTo1(direction, mouseMove.X, fDelta*10.0f);
                }
            }
        } else {
            thisVelocity += gravity * fDelta;
        }
        player.Rotation = new Vector3(player.Rotation.X, FloatTo1(player.Rotation.Y, direction, fDelta*10.0f), player.Rotation.Z);
        // 阻力
        thisVelocity *= 0.95f;
        // 限速
        if (thisVelocity.Length() > 10.0f) {
            thisVelocity = thisVelocity.Normalized() * 10.0f;
        }
        Velocity = thisVelocity;
        // 移动
        MoveAndSlide();
        if (playerState == State.move) {
            cameraManager.UpdateCamera(fDelta, player);
        }
        cameraMarker.Position = cameraShake.GetShakeOffset(ui.totalGameTime) + CameraMarkerOrigin;
        mouseMove = Vector2.Zero;
        right = 0;
        front = 0;
        jump = false;
        isSlow = false;
        if (!backgroundMusic.Playing) {
            backgroundMusic.Playing = true;
        }
    }
    public override void _Input(InputEvent @event) {
        if (@event is InputEventScreenDrag drag) {
            if (ui.uiType == UiType.phone) {
                if (IsInArea(ControlPanel, drag.Position)) {
                    Vector2 moveVector = (drag.Position-ControlPanel.GetGlobalRect().Position-ControlPanel.GetGlobalRect().Size/2).Normalized();
                    right = -moveVector.X;
                    front = moveVector.Y;
                } else {
                    canTurn = true;
                    mouseMove = -drag.Relative * mouseSpeed;
                }
                return;
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
                if (button.Pressed){
                    if(PlayerState is State.move) {
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
            jump = true;
        }
    }
    public void Shake() {
        cameraShake.StartShake(ui.totalGameTime, 200);
    }
    // 用于将from平滑地移动到to，速度为speed，但不会超过to，返回新的from，from和to都是弧度
    public static float FloatTo1(float from, float to, float speed=0.1f) {
        float PI2 = 2.0f*MathF.PI;
        float newTo = to - Mathf.Floor(to / PI2) * PI2;
        newTo += Mathf.Floor(from / PI2) * PI2;
        if (MathF.Abs(newTo + PI2 - from) < MathF.Abs(newTo - from)) {
            newTo += PI2;
        }
        if (MathF.Abs(newTo - PI2 - from) < MathF.Abs(newTo - from)) {
            newTo -= PI2;
        }
        if (newTo > from) {
            return MathF.Min(from+speed, newTo);
        } else {
            return MathF.Max(from-speed, newTo);
        }
    }
    // 位置是否在指定范围内
    public static bool IsInArea(Control area, Vector2 position) {
        return area.GetGlobalRect().HasPoint(position);
    }
}
