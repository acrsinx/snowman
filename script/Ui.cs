using System;
using Godot;
using Godot.Collections;

public partial class Ui : Control {
    [Export] public Camera playerCamera;
    public UiType uiType;
    public Label infomation;
    public PanelContainer captionContainer;
    public Label speakerLabel;
    public Label captionLabel;
    public VBoxContainer chooseBox;
    public Button[] chooseButtons;
    public Control phoneControl;
    public Panel ControlPanel;
    public Button phoneJump;
    public Button phoneAttack;
    public Button phoneSlow;
    public HBoxContainer rightUp;
    public Button setting;
    public Setting settingPanel;
    public Button package;
    public Package packagePanel;
    public Load loadPanel;
    /// <summary>
    /// 左上角的信息
    /// </summary>
    public Control leftUp;
    /// <summary>
    /// 小地图的框
    /// </summary>
    public Panel panel;
    /// <summary>
    /// 小地图
    /// </summary>
    public Sprite2D map;
    public ProgressBar healthBar;
    /// <summary>
    /// 游玩总时长，单位为(ms)，注意这可能会溢出，不过谁会玩这么久呢？
    /// </summary>
    public long totalGameTime = 0;
    private int captionTime = 0;
    private long captionStartTime = 0;
    public CaptionResource[] captions;
    private int captionIndex = 0;
    public void Log(string s) {
        GD.Print("[" + totalGameTime + "] " + s);
    }
    public override void _Ready() {
        // 设备类型
        if (OS.GetName() == "Android" || OS.GetName() == "iOS"){
            uiType = UiType.phone;
        } else if (OS.GetName() == "Windows" || OS.GetName() == "macOS" || OS.GetName() == "Linux") {
            uiType = UiType.computer;
        } else {
            uiType = UiType.computer;
        }
        // 获取组件
        infomation = GetNode<Label>("infomation");
        captionContainer = GetNode<PanelContainer>("CaptionContainer");
        speakerLabel = GetNode<Label>("CaptionContainer/VBoxContainer/SpeakerLabel");
        captionLabel = GetNode<Label>("CaptionContainer/VBoxContainer/CaptionLabel");
        chooseBox = GetNode<VBoxContainer>("choose");
        chooseButtons = new Button[3];
        chooseButtons[0] = GetNode<Button>("choose/Button");
        chooseButtons[1] = GetNode<Button>("choose/Button2");
        chooseButtons[2] = GetNode<Button>("choose/Button3");
        phoneControl = GetNode<Control>("Control");
        ControlPanel = GetNode<Panel>("Control/ControlPanel");
        phoneJump = GetNode<Button>("Control/jump");
        phoneAttack = GetNode<Button>("Control/attack");
        phoneSlow = GetNode<Button>("Control/slow");
        rightUp = GetNode<HBoxContainer>("RightUp");
        setting = GetNode<Button>("RightUp/setting");
        settingPanel = GetNode<Setting>("Setting");
        package = GetNode<Button>("RightUp/package");
        packagePanel = GetNode<Package>("Package");
        loadPanel = GetNode<Load>("Load");
        leftUp = GetNode<Control>("LeftUp");
        panel = GetNode<Panel>("LeftUp/Panel");
        map = GetNode<Sprite2D>("LeftUp/Panel/Map");
        healthBar = GetNode<ProgressBar>("RightUp/health");
        healthBar.Visible = false;
        // 添加事件
        chooseButtons[0].GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    Choose(0);
                }
            }
        };
        chooseButtons[1].GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    Choose(1);
                }
            }
        };
        chooseButtons[2].GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    Choose(2);
                }
            }
        };
        phoneJump.GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    playerCamera.Jump();
                }
            }
        };
        phoneAttack.GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    playerCamera.playerCharacter.Attack();
                }
            }
        };
        setting.GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    Setting();
                }
            }
        };
        package.GuiInput += (InputEvent @event) => {
            if (@event is InputEventScreenTouch touch) {
                if (touch.Pressed) {
                    Package();
                }
            }
        };

        playerCamera.ControlPanel = ControlPanel;
        settingPanel.ui = this;
        packagePanel.ui = this;
        loadPanel.ui = this;
        settingPanel.Init();
        packagePanel.Init();
        loadPanel.Init();
        ClearChoose();
        // 设置为加载态，前面的ClearCaption();会把playerCamera.PlayerState设为State.move
        playerCamera.PlayerState = State.load;
    }
    public override void _Process(double delta) {
        if (settingPanel.showInfo.ButtonPressed) {
            infomation.Text = "fps: " + Engine.GetFramesPerSecond()
            + ", 最大fps: " + Engine.MaxFps
            + ", 每秒处理数: " + (1/delta)
            + "\n物理每秒处理数: " + Engine.PhysicsTicksPerSecond
            + "\nposition: (" + MathF.Round(playerCamera.player.GlobalPosition.X) + ", " + MathF.Round(playerCamera.player.GlobalPosition.Y) + ", " + MathF.Round(playerCamera.player.GlobalPosition.Z) + ")"
            + ", state: " + playerCamera.PlayerState.ToString()
            + ", uiType: " + uiType.ToString()
            + ", LOD: " + GetTree().Root.MeshLodThreshold
            + "\ntime: " + totalGameTime
            + ", health: " + playerCamera.playerCharacter?.health
            + "\n用户数据目录: " + OS.GetUserDataDir();
        }
        totalGameTime += (long)(delta * 1e3);
        if (playerCamera.PlayerState == State.caption) {
            // 计时器累加
            if (totalGameTime - captionStartTime <= captionTime) {
                captionLabel.VisibleRatio = (float)(totalGameTime - captionStartTime) / captionTime;
                return;
            }
            // 计时器结束，显示全部字符
            captionLabel.VisibleCharacters = -1;
        }
        if (playerCamera.PlayerState == State.move) {
            // 更新小地图
            map.Position = Map.GlobalPositionToMapPosition(playerCamera, Vector3.Zero);
            map.Scale = new Vector2(1.0f, 1.0f);
        }
    }
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton button) {
            if (button.Pressed) {
                if (button.ButtonIndex == MouseButton.Left) {
                    NextCaption();
                }
            }
        } else if (@event is InputEventKey key) {
            if (key.Pressed) {
                if (key.Keycode == Key.Space) {
                    NextCaption();
                }
            }
        }
    }
    public override void _Notification(int what) {
        if (what == NotificationWMCloseRequest) { // 关闭窗口
            Log("exit");
            // FileAccess file = FileAccess.Open("user://userData.txt", FileAccess.ModeFlags.Write);
            // file.Close();
            GetTree().Quit();
        }
    }
    /// <summary>
    /// 跳过对话或开始选择
    /// </summary>
    public void NextCaption() {
        if (playerCamera.PlayerState != State.caption) {
            return;
        }
        DisplayServer.TtsStop();
        if (totalGameTime - captionStartTime < captionTime) { // 如果文字还没显示完，让文字直接显示完
            captionStartTime = totalGameTime - captionTime;
            return;
        }
        if (captions[captionIndex].canChoose) { // 如果可以选择
            if (!chooseButtons[0].Visible) { // 如果还没显示选择按钮
                ShowCaptionChoose(captionIndex);
            }
        } else { // 如果不需要选择，即普通对话，即可跳过
            Plot.ParseScript(captions[captionIndex].endCode);
        }
    }
    public void Choose(int index) {
        if (playerCamera.PlayerState != State.caption) { // 如果不在对话态，提前返回
            return;
        }
        if (captions[captionIndex].canChoose && chooseButtons[0].Visible && index < captions[captionIndex].choose.Length && index >= 0) { // 可选
            // 清空选项
            ClearChoose();
            // 执行选择后的脚本
            Plot.ParseScript(captions[captionIndex].chooseEndCode[index]);
        }
    }
    public void ShowCaption(Dictionary dict) {
        if (playerCamera.PlayerState != State.caption) {
            int i = 0;
            captions = new CaptionResource[dict.Count];
            while(dict.ContainsKey(i.ToString())) {
                captions[i] = new(this, (Dictionary) dict[i.ToString()], i);
                i++;
            }
            ShowCaption(0);
        }
    }
    public void ShowCaption(int id) {
        captionIndex = id;
        SetCaption(captions[id].actorName, captions[id].caption, captions[id].time);
        Plot.ParseScript(captions[id].startCode);
    }
    public void ShowCaptionChoose(int id) {
        chooseBox.Visible = true;
        for (int i = 0; i < captions[captionIndex].choose.Length; i++) {
            chooseButtons[i].Visible = true;
            chooseButtons[i].Text = captions[id].choose[i];
        }
    }
    private void SetCaption(string speakerName, string caption, int time) {
        playerCamera.PlayerState = State.caption;
        captionStartTime = totalGameTime;
        speakerLabel.Text = speakerName;
        captionLabel.Text = caption;
        captionTime = time;
        if (settingPanel.ttsId == "") {
            return;
        }
        DisplayServer.TtsSpeak(caption, settingPanel.ttsId);
    }
    public void ClearChoose() {
        chooseBox.Visible = false;
        for (int i = 0; i < 3; i++) {
            chooseButtons[i].Visible = false;
        }
        playerCamera.PlayerState = State.move;
    }
    public void Setting() {
        if (playerCamera.PlayerState != State.setting) {
            playerCamera.PlayerState = State.setting;
        } else if (playerCamera.PlayerState == State.setting) {
            playerCamera.PlayerState = State.move;
        }
    }
    public void Package() {
        if (playerCamera.PlayerState != State.package) {
            playerCamera.PlayerState = State.package;
        } else if (playerCamera.PlayerState == State.package) {
            playerCamera.PlayerState = State.move;
        }
    }
    public void Exit() {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
    public bool CanUse(GameStuff gameStuff) {
        return gameStuff.CanUse();
    }
    public void Use(GameStuff gameStuff) {
        gameStuff.Use();
    }
}
