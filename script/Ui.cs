using Godot;
using Godot.Collections;
public partial class Ui: Control {
    public const string savePath = "user://save.json";
    public Player player;
    public Label infomation;
    public PanelContainer captionContainer;
    public Label speakerLabel;
    public Label captionLabel;
    public VBoxContainer chooseBox;
    public Button[] chooseButtons;
    public Control phoneControl;
    public Panel ControlPanel;
    public TouchScreenButton phoneJump;
    public TouchScreenButton phoneAttack;
    public TouchScreenButton phoneSlow;
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
    /// <summary>
    /// 任务指引
    /// </summary>
    public Label task;
    private string taskString = "";
    /// <summary>
    /// 任务指引，请赋值本地化前的，取值时为本地化后的
    /// </summary>
    public string TaskString {
        get => taskString;
        set {
            taskString = value;
            task.Text = Translation.Translate(taskString, Plot.PlotPathToLocalizationContent(Plot.path));
        }
    }
    public ProgressBar healthBar;
    /// <summary>
    /// 游玩总时长，单位为(ms)，注意这可能会溢出，不过谁会玩这么久呢？
    /// </summary>
    public static long totalGameTime = 0;
    private int captionTime = 0;
    private long captionStartTime = 0;
    public CaptionResource[] captions;
    private int captionIndex = 0;
    public int CaptionIndex {
        get => captionIndex;
    }
    private static readonly string[] Logs = new string[3];
    public static void Log(string s) {
        string toLog = "[" + totalGameTime + "] " + s;
        Logs[0] = Logs[1];
        Logs[1] = Logs[2];
        Logs[2] = toLog;
        GD.Print(toLog);
    }
    public static void Log(params object[] objects) {
        if (objects == null || objects.Length == 0) {
            return;
        }
        string s = "";
        foreach (var o in objects) {
            s += o.ToString() + " ";
        }
        Log(s);
    }
    public void Init(Setting settingPanel, Player player) {
        settingPanel.Reparent(this);
        this.settingPanel = settingPanel;
        this.player = player;
        // 获取组件
        infomation = GetNode<Label>("infomation");
        settingPanel.gameInformation.gameInformation = infomation;
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
        phoneJump = GetNode<TouchScreenButton>("Control/RightDown/jump");
        phoneAttack = GetNode<TouchScreenButton>("Control/RightDown/attack");
        phoneSlow = GetNode<TouchScreenButton>("Control/RightDown/slow");
        rightUp = GetNode<HBoxContainer>("RightUp");
        setting = GetNode<Button>("RightUp/setting");
        package = GetNode<Button>("RightUp/package");
        packagePanel = GetNode<Package>("Package");
        loadPanel = GetNode<Load>("Load");
        leftUp = GetNode<Control>("LeftUp");
        panel = GetNode<Panel>("LeftUp/Panel");
        map = GetNode<Sprite2D>("LeftUp/Panel/Map");
        task = GetNode<Label>("LeftUp/Task");
        healthBar = GetNode<ProgressBar>("RightUp/health");
        Light3D light = player.root.GetNode<Light3D>("sunLight");
        if (light is null) {
            Log("找不到灯光。");
        }
        settingPanel.gameInformation.light = light;
        healthBar.Visible = false;
        packagePanel.Init(this);
        loadPanel.Init(this);
        // 添加事件
        chooseButtons[0].GuiInput += @event => {
            if (@event is InputEventScreenTouch touch) {
                if (!touch.Pressed) {
                    Choose(0);
                }
            }
        };
        chooseButtons[1].GuiInput += @event => {
            if (@event is InputEventScreenTouch touch) {
                if (!touch.Pressed) {
                    Choose(1);
                }
            }
        };
        chooseButtons[2].GuiInput += @event => {
            if (@event is InputEventScreenTouch touch) {
                if (!touch.Pressed) {
                    Choose(2);
                }
            }
        };
        setting.GuiInput += @event => {
            if (@event is InputEventScreenTouch touch) {
                if (!touch.Pressed) {
                    player.PlayerState = State.setting;
                }
            }
        };
        settingPanel.GetNodeButton("back").Pressed += () => {
            player.PlayerState = State.move;
        };
        package.GuiInput += @event => {
            if (@event is InputEventScreenTouch touch) {
                if (!touch.Pressed) {
                    Package();
                }
            }
        };
        Translation.LangageChanged += () => {
            phoneJump.GetChild<Label>(0).Text = Translation.Translate("跳");
            phoneAttack.GetChild<Label>(0).Text = Translation.Translate("攻");
            phoneSlow.GetChild<Label>(0).Text = Translation.Translate("慢");
            package.Text = Translation.Translate("包");
            setting.Text = Translation.Translate("设");
            task.Text = TaskString;
        };
        ClearChoose();
        // 设置为加载态，前面的ClearCaption();会把player.PlayerState设为State.move
        player.PlayerState = State.load;
    }
    public override void _Process(double delta) {
        if (settingPanel.gameInformation.ShowInfo) {
            string text = "fps: " + Engine.GetFramesPerSecond() + ", DrawCalls: " + Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame) + ", 最大fps: " + Engine.MaxFps + ", 每秒处理数: " + (1 / delta) + "\n物理每秒处理数: " + Engine.PhysicsTicksPerSecond + ", state: " + player.PlayerState.ToString() + ", uiType: " + settingPanel.gameInformation.UiType.ToString() + ", 语言: " + Translation.Locale + "\ntime: " + totalGameTime + ", health: " + player.character?.health + "\n用户数据目录: " + OS.GetUserDataDir();
            text += "\n" + Logs[0];
            text += "\n" + Logs[1];
            text += "\n" + Logs[2];
            if (player.PlayerState == State.caption) {
                text += "\n剧情位置: " + Plot.path + ":" + captionIndex.ToString();
            }
            infomation.Text = text;
        }
        if (player.PlayerState == State.caption) {
            if (totalGameTime - captionStartTime <= captionTime) {
                captionLabel.VisibleRatio = (float)(totalGameTime - captionStartTime) / captionTime;
                return;
            }
            // 计时器结束，显示全部字符
            captionLabel.VisibleCharacters = -1;
        }
        if (player.PlayerState == State.move) {
            // 更新小地图
            map.Position = Map.GlobalPositionToMapPosition(player, Vector3.Zero);
            map.Scale = new Vector2(1.0f, 1.0f);
        }
    }
    public override void _PhysicsProcess(double delta) {
        // 计时器累加
        totalGameTime += (long)(delta * 1e3);
        // 更新相机动画
        player.cameraManager.PosesAnimation();
    }
    public override void _Input(InputEvent @event) {
        if (@event.IsAction("show_info")) {
            if (@event.IsReleased()) {
                return;
            }
            // 打开或关闭调试信息
            settingPanel.gameInformation.ShowInfo = !settingPanel.gameInformation.ShowInfo;
            return;
        }
        if (@event.IsAction("next_caption")) {
            if (@event.IsReleased()) {
                return;
            }
            NextCaption();
            return;
        }
        if (@event.IsAction("jump")) {
            if (@event.IsReleased()) {
                return;
            }
            player.Jump();
            return;
        }
        if (@event.IsAction("attack")) {
            if (@event.IsReleased()) {
                return;
            }
            player.character.Attack();
            return;
        }
    }
    /// <summary>
    /// 跳过对话或开始选择
    /// </summary>
    public void NextCaption() {
        if (player.PlayerState != State.caption) {
            return;
        }
        if (totalGameTime - captionStartTime < captionTime) { // 如果文字还没显示完，让文字直接显示完
            captionStartTime = totalGameTime - captionTime;
            return;
        }
        DisplayServer.TtsStop();
        if (captions[captionIndex].canChoose) { // 如果可以选择
            if (!chooseButtons[0].Visible) { // 如果还没显示选择按钮
                ShowCaptionChoose(captionIndex);
            }
        } else { // 如果不需要选择，即普通对话，即可跳过
            player.cameraManager.PauseCameraAnimation();
            Plot.ParseScript(captions[captionIndex].endCode);
        }
    }
    public void Choose(int index) {
        if (player.PlayerState != State.caption) { // 如果不在对话态，提前返回
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
        if (player.PlayerState != State.caption) {
            int i = 0;
            captions = new CaptionResource[dict.Count];
            while (dict.ContainsKey(i.ToString())) {
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
        // 设置相机动画
        if (captions[id].endCode == null) {
            return;
        }
        player.cameraManager.SetPosesAnimationTime(captions[id].time);
        player.cameraManager.PushCurrentCameraPose();
        Plot.ParseCameraScript(captions[id].endCode);
        player.cameraManager.PushCurrentCameraPose();
        player.cameraManager.PosesAnimation();
    }
    public void ShowCaptionChoose(int id) {
        chooseBox.Visible = true;
        for (int i = 0; i < captions[captionIndex].choose.Length; i++) {
            chooseButtons[i].Visible = true;
            chooseButtons[i].Text = captions[id].choose[i];
        }
    }
    private void SetCaption(string speakerName, string caption, int time) {
        player.PlayerState = State.caption;
        captionStartTime = totalGameTime;
        speakerLabel.Text = Translation.Translate(speakerName, "character");
        captionLabel.Text = Translation.Translate(caption, Plot.PlotPathToLocalizationContent(Plot.path));
        captionTime = time;
        if (settingPanel.ttsId == "") {
            return;
        }
        // 使用TTS读出台词
        string ttsTranslate = Translation.Translate(caption, Plot.PlotPathToLocalizationContent(Plot.path), (string) settingPanel.voices[settingPanel.voiceIndex]["language"]);
        DisplayServer.TtsSpeak(ttsTranslate, settingPanel.ttsId);
    }
    public void ClearChoose() {
        chooseBox.Visible = false;
        for (int i = 0; i < 3; i++) {
            chooseButtons[i].Visible = false;
        }
        player.PlayerState = State.move;
    }
    public void Package() {
        if (player.PlayerState != State.package) {
            player.PlayerState = State.package;
        } else if (player.PlayerState == State.package) {
            player.PlayerState = State.move;
        }
    }
    public bool CanUse(GameStuff gameStuff) {
        return gameStuff.CanUse();
    }
    public void Use(GameStuff gameStuff) {
        gameStuff.Use();
    }
}
