using Godot;
using Godot.Collections;

public class Plot {
    public static string[] paths;
    public static void Check(Ui ui) {
        Plot plot = new Plot0_0();
        plot.Open(ui);
    }
    public void Open(Ui ui, int n) {
        if (FileAccess.FileExists(paths[n])) {
            FileAccess fileAccess = FileAccess.Open(paths[n], FileAccess.ModeFlags.Read);
            ui.ShowCaption((Dictionary) Json.ParseString(fileAccess.GetAsText()));
            fileAccess.Close();
        } else {
            GD.Print("未找到文件: " + paths[n]);
        }
    }
    public void Open(Ui ui) {
        Open(ui, 0);
    }
}