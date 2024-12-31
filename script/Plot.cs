using Godot;
using Godot.Collections;

public class Plot {
    public static string[] paths;
    public static void Check(Ui ui) {
        Plot plot = new Plot0_0();
        plot.Open(ui, plot);
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
    public virtual void Animate(int id, Ui ui) { }
    public void Open(Ui ui, Plot plot) {
        Open(ui, 0, plot);
    }
}