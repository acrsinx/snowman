using System.Collections.Generic;
using Godot;
using Godot.Collections;
public class Plot {
    public static Dictionary CharacterPath = new() {
        {"snowdog", "res://model/snowdog.gltf"}
    };
    public static System.Collections.Generic.Dictionary<string, GameCharacter> InstanceName = new() {
    };
    public static string[] paths;
    public static Player player;
    public static void Check(Ui ui) {
        paths = new string[] {
            "res://plotJson/plot0/plot0_0.json",
            "res://plotJson/plot0/plot0_1.json",
            "res://plotJson/plot0/plot0_2.json",
        };
        Open(ui);
    }
    /// <summary>
    /// 通过名字获取角色
    /// </summary>
    /// <param name="instanceName">角色名字</param>
    /// <returns>返回角色名字</returns>
    public static PlotCharacter GetPlotCharacter(string instanceName) {
        if (!InstanceName.ContainsKey(instanceName)) {
            player.ui.Log("未找到剧情角色：" + instanceName);
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
            player.ui.Log("已存在角色：" + instanceName);
            return;
        }
        if (CharacterPath.ContainsKey(characterName)) {
            PackedScene character = ResourceLoader.Load<PackedScene>((string) CharacterPath[characterName]);
            GameCharacter plotCharacter = new(character, player, false) {
                Position = position
            };
            InstanceName.Add(instanceName, plotCharacter);
            return;
        }
        GameCharacter gameCharacter;
        switch (characterName) {
            case "snowbear": {
                gameCharacter = new Snowbear(player);
                break;
            }
            default: {
                player.ui.Log("未找到角色：" + characterName);
                return;
            }
        }
        gameCharacter.Position = position;
        InstanceName.Add(instanceName, gameCharacter);
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
    /// 解析剧情脚本
    /// </summary>
    public static void ParseScriptLine(string scriptLine) {
        // 去除多余的空格
        scriptLine = scriptLine.Trim();
        // 将","，"("，")"替换为空格
        scriptLine = scriptLine.Replace(",", " ");
        scriptLine = scriptLine.Replace("(", " ");
        scriptLine = scriptLine.Replace(")", " ");
        // 分词
        string[] words = scriptLine.Split(' ');
        // 去除空字符串
        List<string> wordsList = new();
        foreach (string word in words) {
            if (word != "") {
                wordsList.Add(word);
            }
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
            case "SetCameraPosition": {
                SetCameraPosition();
                break;
            }
            case "Goto": {
                player.ui.ShowCaption(int.Parse(wordsList[1]));
                break;
            }
            case "Exit": {
                player.PlayerState = State.move;
                break;
            }
            default: {
                player.ui.Log("未知的剧情指令: " + wordsList[0]);
                break;
            }
        }
    }
    public static void ParseScript(string script) {
        string[] lines = script.Split(';');
        foreach (string line in lines) {
            ParseScriptLine(line);
        }
    }
    public static void Open(Ui ui, int n) {
        if (!FileAccess.FileExists(paths[n])) {
            ui.Log("未找到文件: " + paths[n]);
            return;
        }
        if (paths == null) {
            ui.Log("未设置剧情文件路径");
        }
        FileAccess fileAccess = FileAccess.Open(paths[n], FileAccess.ModeFlags.Read);
        ui.ShowCaption((Dictionary) Json.ParseString(fileAccess.GetAsText()));
        fileAccess.Close();
    }
    public static void Open(Ui ui) {
        Open(ui, 0);
    }
}
