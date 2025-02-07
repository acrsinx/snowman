using Godot;
using Godot.Collections;
/// <summary>
/// 游戏信息类
/// </summary>
public class GameInformation: object {
    public Ui ui;
    public GameInformation(Ui ui) {
        this.ui = ui;
    }
    /// <summary>
    /// 保存游戏信息到指定文件
    /// </summary>
    /// <param name="path">文件路径</param>
    public void SaveInformation(string path) {
        Dictionary<string, string> information = new() {
            {"totalGameTime", ui.totalGameTime.ToString()},
            {"maxFps", ui.settingPanel.maxFps.Selected.ToString()},
            {"tts", ui.settingPanel.tts.Selected.ToString()},
            {"useScreenShader", ui.settingPanel.useScreenShader.ButtonPressed?"1":"0"},
            {"shadow", ui.settingPanel.shadow.ButtonPressed?"1":"0"},
            {"showInfo", ui.settingPanel.showInfo.ButtonPressed?"1":"0"},
            {"LOD", ui.settingPanel.LOD.Value.ToString()}
        };
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreLine(Json.Stringify(information));
        file.Close();
    }
    /// <summary>
    /// 从指定文件读取游戏信息
    /// </summary>
    /// <param name="path">文件路径</param>
    public void LoadInformation(string path) {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null) {
            ui.Log("读取游戏信息失败，文件不存在");
            return;
        }
        Dictionary<string, string> information = (Dictionary<string, string>) Json.ParseString(file.GetAsText());
        ui.totalGameTime = long.Parse(information["totalGameTime"]);
        ui.settingPanel.maxFps.Selected = int.Parse(information["maxFps"]);
        ui.settingPanel.SetMaxFps(int.Parse(information["maxFps"]));
        ui.settingPanel.tts.Selected = int.Parse(information["tts"]);
        ui.settingPanel.SetTtsId(int.Parse(information["tts"]));
        ui.settingPanel.useScreenShader.ButtonPressed = information["useScreenShader"] == "1";
        ui.settingPanel.SetUseScreenShader();
        ui.settingPanel.shadow.ButtonPressed = information["shadow"] == "1";
        ui.settingPanel.SetShadow();
        ui.settingPanel.showInfo.ButtonPressed = information["showInfo"] == "1";
        ui.settingPanel.SetShowInfo();
        ui.settingPanel.LOD.Value = double.Parse(information["LOD"]);
        ui.settingPanel.SetLOD(double.Parse(information["LOD"]));
        file.Close();
    }
}
