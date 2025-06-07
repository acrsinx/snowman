using System.Collections.Generic;
using Godot;
using Godot.Collections;
public class Plot {
    public static readonly Dictionary CharacterPath = new() {
        {
            "snowdog",
            "res://model/snowdog.gltf"
        }
    };
    public static System.Collections.Generic.Dictionary<string, GameCharacter> InstanceName = new() {
    };
    /// <summary>
    /// 剧情文件路径
    /// </summary>
    public static string path = "";
    public static Player player;
    public static void Check(Ui ui) {
        path = "res://plotJson/plot0/plot0_0.json";
        Open(ui);
    }
    /// <summary>
    /// 通过名字获取角色
    /// </summary>
    /// <param name="instanceName">角色名字</param>
    /// <returns>返回角色名字</returns>
    public static PlotCharacter GetPlotCharacter(string instanceName) {
        if (!InstanceName.ContainsKey(instanceName)) {
            Ui.Log("未找到剧情角色：" + instanceName);
        }
        return InstanceName[instanceName];
    }
    /// <summary>
    /// 加载角色到场景中
    /// </summary>
    /// <param name="characterName">角色类名字</param>
    /// <param name="instanceName">角色对象名字</param>
    /// <param name="position">角色位置</param>
    public static void LoadCharacter(string characterName, string instanceName, Vector3 position) {
        if (InstanceName.ContainsKey(instanceName)) {
            Ui.Log("已存在角色：" + instanceName);
            return;
        }
        if (CharacterPath.ContainsKey(characterName)) {
            PackedScene character = ResourceLoader.Load<PackedScene>((string) CharacterPath[characterName]);
            GameCharacter plotCharacter = new(character, player, new SphereShape3D() {
                Radius = 0.25f
            }, new Vector3(0, 0.125f, 0), false) {
                Position = position
            };
            AddCharacterInstance(instanceName, plotCharacter);
            return;
        }
        GameCharacter gameCharacter;
        switch (characterName) {
            case "snowman": {
                gameCharacter = new Snowman(player, false);
                break;
            }
            case "snowbear": {
                gameCharacter = new Snowbear(player);
                break;
            }
            default: {
                Ui.Log("未找到角色：" + characterName);
                return;
            }
        }
        gameCharacter.Position = position;
        AddCharacterInstance(instanceName, gameCharacter);
    }
    /// <summary>
    /// 添加角色到实例列表中
    /// </summary>
    /// <param name="instanceName">角色名</param>
    /// <param name="gameCharacter">角色实例</param>
    public static void AddCharacterInstance(string instanceName, GameCharacter gameCharacter) {
        InstanceName.Add(instanceName, gameCharacter);
        gameCharacter.Name = instanceName;
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="instanceName">角色名</param>
    /// <param name="animationName">动画名</param>
    public static void PlayAnimation(string instanceName, string animationName) {
        GetPlotCharacter(instanceName).PlayAnimation(animationName);
    }
    /// <summary>
    /// 暂停动画
    /// </summary>
    /// <param name="instanceName">角色名</param>
    public static void PauseAnimation(string instanceName) {
        GetPlotCharacter(instanceName).PauseAnimation();
    }
    /// <summary>
    /// 看向角色
    /// </summary>
    /// <param name="instanceName">角色名</param>
    /// <param name="height">相机高度</param>
    /// <param name="distance">距离</param>
    public static void LookAtCharacter(string instanceName, float height, float distance) {
        player.cameraManager.LookAtCharacter(GetPlotCharacter(instanceName), height, distance);
    }
    /// <summary>
    /// 设置回相机位置
    /// </summary>
    public static void SetCameraPosition() {
        player.cameraManager.SetCameraPosition();
    }
    /// <summary>
    /// 判断是否是相机脚本
    /// </summary>
    /// <param name="word">剧情指令</param>
    /// <returns>是否是</returns>
    public static bool IsCameraScriptLine(string word) {
        switch (word) {
            case "LookAtCharacter":
            case "SetCameraPosition": {
                return true;
            }
            default: {
                return false;
            }
        }
    }
    /// <summary>
    /// 分词
    /// </summary>
    /// <param name="scriptLine">剧情脚本</param>
    /// <returns>词</returns>
    public static List<string> SplitWord(string scriptLine) {
        // 处理大括号
        scriptLine = scriptLine.Replace("{", " {");
        scriptLine = scriptLine.Replace("}", "} ");
        // 去除多余的空格
        scriptLine = scriptLine.Trim();
        // 将","，"("，")"替换为空格
        scriptLine = scriptLine.Replace(",", " ");
        scriptLine = scriptLine.Replace("(", " ");
        scriptLine = scriptLine.Replace(")", " ");
        // 分词
        List<string> wordsList = new();
        int lastIndex = 0;
        int tabLevel = 0;
        for (int i = 0; i < scriptLine.Length; i++) {
            if (scriptLine[i] == ' ' && tabLevel == 0) {
                wordsList.Add(scriptLine[lastIndex..i]);
                lastIndex = i + 1;
                continue;
            }
            if (scriptLine[i] == '{') {
                tabLevel++;
                continue;
            }
            if (scriptLine[i] == '}') {
                tabLevel--;
                continue;
            }
        }
        wordsList.Add(scriptLine[lastIndex..]);
        return wordsList;
    }
    /// <summary>
    /// 解析剧情脚本
    /// </summary>
    public static void ParseScriptLine(List<string> wordsList) {
        if (wordsList.Count == 0) {
            return;
        }
        // 解析核心词
        switch (wordsList[0]) {
            case "LoadCharacter": {
                LoadCharacter(wordsList[1], wordsList[2], new Vector3(float.Parse(wordsList[3]), float.Parse(wordsList[4]), float.Parse(wordsList[5])));
                break;
            }
            case "PlayAnimation": {
                PlayAnimation(wordsList[1], wordsList[2]);
                break;
            }
            case "PauseAnimation": {
                PauseAnimation(wordsList[1]);
                break;
            }
            case "LookAtCharacter": {
                LookAtCharacter(wordsList[1], float.Parse(wordsList[2]), float.Parse(wordsList[3]));
                break;
            }
            case "PlayerTo": {
                player.character.GlobalPosition = new Vector3(float.Parse(wordsList[1]), float.Parse(wordsList[2]), float.Parse(wordsList[3]));
                SetCameraPosition();
                break;
            }
            case "SetCameraPosition": {
                SetCameraPosition();
                break;
            }
            case "SetTaskName": {
                player.ui.TaskString = wordsList[1];
                break;
            }
            case "Goto": {
                player.ui.ShowCaption(player.ui.CaptionIndex + int.Parse(wordsList[1]));
                break;
            }
            case "AddTrigger": {
                TriggerSystem.AddTrigger(wordsList[1], () => {
                    ParseScript(wordsList[2]);
                });
                break;
            }
            case "Jump": {
                path = PlotPathToAbsolutePath(wordsList[1]);
                Open(player.ui);
                break;
            }
            case "Exit": {
                player.PlayerState = State.move;
                break;
            }
            default: {
                Ui.Log("未知的剧情指令: " + wordsList[0]);
                break;
            }
        }
    }
    public static void ParseScript(string script) {
        if (script == "" || script == null) {
            return;
        }
        if (script[0] == '{' && script[^1] == '}') {
            ParseScript(script[1..^1]);
            return;
        }
        List<string> lines = new();
        int lastIndex = 0;
        int tabLevel = 0;
        for (int i = 0; i < script.Length; i++) {
            if (script[i] == ';' && tabLevel == 0) {
                lines.Add(script[lastIndex..i]);
                lastIndex = i + 1;
                continue;
            }
            if (script[i] == '{') {
                tabLevel++;
                continue;
            }
            if (script[i] == '}') {
                tabLevel--;
                if (tabLevel < 0) {
                    Ui.Log(path + "语法错误: " + script);
                    return;
                }
                continue;
            }
        }
        if (lastIndex < script.Length) {
            lines.Add(script[lastIndex..]);
        }
        foreach (string line in lines) {
            ParseScriptLine(SplitWord(line));
        }
    }
    public static void ParseCameraScript(string script) {
        string[] lines = script.Split(';');
        foreach (string line in lines) {
            List<string> wordsList = SplitWord(line);
            if (IsCameraScriptLine(wordsList[0])) {
                ParseScriptLine(wordsList);
            }
        }
    }
    public static void Open(Ui ui) {
        if (path == null) {
            Ui.Log("未设置剧情文件路径");
        }
        if (!FileAccess.FileExists(path)) {
            Ui.Log("未找到文件: " + path);
            return;
        }
        FileAccess fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        ui.ShowCaption((Dictionary) Json.ParseString(fileAccess.GetAsText()));
        fileAccess.Close();
    }
    /// <summary>
    /// 把剧情文件路径转换为本地化所用的上下文
    /// </summary>
    /// <param name="path">剧情文件路径</param>
    /// <returns>本地化文件的上下文</returns>
    public static string PlotPathToLocalizationContent(string path) {
        return "plot/" + path.Replace("\\", "/").Replace("res://", "").Replace("plotJson/", "").Replace(".json", "").Replace("/", "_");
    }
    /// <summary>
    /// 把相对plotJson文件夹的剧情文件路径转换为绝对路径
    /// </summary>
    /// <param name="path">相对plotJson文件夹的剧情文件路径</param>
    /// <returns>绝对路径</returns>
    public static string PlotPathToAbsolutePath(string path) {
        return "res://plotJson/" + path.Replace("\\", "/");
    }
}
