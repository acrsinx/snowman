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
        InstanceName.Add(instanceName, characterIns);
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