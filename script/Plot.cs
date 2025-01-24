using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Plot {
    public static Dictionary CharacterPath = new() {
        {"snowdog", "res://model/snowdog.gltf"}
    };
    public static System.Collections.Generic.Dictionary<string, object> InstanceName = new() {};
    public static string[] paths;
    public static Camera camera;
    public static void Check(Ui ui) {
        Plot plot = new Plot0_0();
        plot.Open(ui, plot);
    }
    /// <summary>
    /// 通过名字获取角色
    /// </summary>
    /// <param name="instanceName">角色名字</param>
    /// <returns>返回角色名字</returns>
    public static PlotCharacter GetPlotCharacter(string instanceName) {
        if (!InstanceName.ContainsKey(instanceName)) {
            camera.ui.Log("未找到剧情角色："+instanceName);
        }
        return (PlotCharacter) InstanceName[instanceName];
    }
    /// <summary>
    /// 加载角色到场景中
    /// </summary>
    /// <param name="characterName">角色类名字</param>
    /// <param name="instanceName">角色对象名字</param>
    /// <param name="position">角色位置</param>
    public static void LoadCharacter(string characterName, string instanceName, Vector3 position) {
        PackedScene character = ResourceLoader.Load<PackedScene>((string) CharacterPath[characterName]);
        Node3D characterIns = character.Instantiate<Node3D>();
        characterIns.Position = position;
        camera.GetParent().AddChild(characterIns);
        PlotCharacter plotCharacter = new(characterIns);
        InstanceName.Add(instanceName, plotCharacter);
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
        camera.cameraManager.LookAtCharacter(GetPlotCharacter(instanceName), height, distance);
    }
    /// <summary>
    /// 设置回相机位置
    /// </summary>
    public static void SetCameraPosition() {
        camera.cameraManager.SetCameraPosition();
    }
    /// <summary>
    /// 解析剧情脚本
    /// </summary>
    public void ParseScriptLine(string scriptLine) {
        // 去除多余的空格
        scriptLine = scriptLine.Trim();
        // 将","，"("，")"替换为空格
        scriptLine = scriptLine.Replace(",", " ");
        scriptLine = scriptLine.Replace("(", " ");
        scriptLine = scriptLine.Replace(")", " ");
        // 分词
        string[] words = scriptLine.Split(' ');
        // 去除空字符串

        for (int i = 0; i < words.Length; i++) {
            GD.Print(words[i]);
        }

        List<string> wordsList = new();
        foreach (string word in words) {
            if (word != "") {
                wordsList.Add(word);
            }
        }

        for (int i = 0; i < words.Length; i++) {
            GD.Print(words[i]);
        }

        // 解析核心词
        switch (wordsList[0]) {
            case "LoadCharacter":
                LoadCharacter(wordsList[1], wordsList[2], new Vector3(float.Parse(wordsList[3]), float.Parse(wordsList[4]), float.Parse(wordsList[5])));
                break;
            case "PlayAnimation":
                PlayAnimation(wordsList[1], wordsList[2]);
                break;
            case "PauseAnimation":
                PauseAnimation(wordsList[1]);
                break;
            case "LookAtCharacter":
                LookAtCharacter(wordsList[1], float.Parse(wordsList[2]), float.Parse(wordsList[3]));
                break;
            case "SetCameraPosition":
                SetCameraPosition();
                break;
            default:
                camera.ui.Log("未知的剧情指令: " + wordsList[0]);
                break;
        }
    }
    public void Open(Ui ui, int n, Plot plot) {
        if (FileAccess.FileExists(paths[n])) {
            FileAccess fileAccess = FileAccess.Open(paths[n], FileAccess.ModeFlags.Read);
            ui.ShowCaption((Dictionary) Json.ParseString(fileAccess.GetAsText()), plot);
            fileAccess.Close();
        } else {
            ui.Log("未找到文件: " + paths[n]);
        }
    }
    public virtual void Animate(int id, Ui ui, bool isEnd, int code) { }
    public void Open(Ui ui, Plot plot) {
        Open(ui, 0, plot);
    }
}