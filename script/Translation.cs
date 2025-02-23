using Godot;
using Godot.Collections;
public class Translation: object {
    private static string locale = "";
    public static string Locale {
        get {
            return locale;
        }
        set {
            if (locale == value) {
                return;
            }
            locale = value;
            Rollback();
        }
    }
    /// <summary>
    /// 语言改变时触发
    /// </summary>
    public static Tool.Void LangageChanged = () => {
    };
    public static Dictionary<string, Dictionary<string, string>> loaded = new();
    public static string[] GetLanguages() {
        string[] list = DirAccess.Open(OS.GetUserDataDir() + "/localization/").GetDirectories();
        return list;
    }
    /// <summary>
    /// 自动回滚语言
    /// </summary>
    private static void Rollback() {
        if (locale == null) {
            locale = "";
            LangageChanged.Invoke();
            return;
        }
        if (locale == "") {
            LangageChanged.Invoke();
            return;
        }
        if (DirAccess.DirExistsAbsolute(GetDir())) {
            LangageChanged.Invoke();
            return;
        }
        // 不存在该语言文件，回滚
        // zh_HK -> zh
        locale = locale.Split("_")[0];
        if (DirAccess.DirExistsAbsolute(GetDir())) {
            LangageChanged.Invoke();
            return;
        }
        // 回滚失败，不翻译
        locale = "";
        LangageChanged.Invoke();
    }
    public static string GetDir() {
        return OS.GetUserDataDir() + "/localization/" + Locale + "/";
    }
    public static string Translate(string source, string content = "core") {
        if (Locale == "") {
            return source;
        }
        if (source == "") {
            return source;
        }
        string fileName = GetDir() + content + ".json";
        // 已经加载过该文件
        if (loaded.ContainsKey(fileName)) {
            if (loaded[fileName].ContainsKey(source)) {
                return loaded[fileName][source];
            }
            Ui.LogStatic("找不到翻译: ", source, ", 文件: ", fileName);
            return source;
        }
        // 尝试加载文件
        if (!FileAccess.FileExists(fileName)) {
            Ui.LogStatic("找不到翻译文件: ", fileName);
            return source;
        }
        // 打开文件
        FileAccess file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read);
        if (file == null) {
            Ui.LogStatic("无法打开翻译文件: ", fileName);
            return source;
        }
        string json = file.GetAsText();
        file.Close();
        // 解析文件
        Dictionary<string, string> f = (Dictionary<string, string>) Json.ParseString(json);
        loaded.Add(fileName, f);
        if (f.ContainsKey(source)) {
            return f[source];
        }
        Ui.LogStatic("找不到翻译: ", source, ", 文件: ", fileName);
        return source;
    }
}
