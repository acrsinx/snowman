using Godot;
using Godot.Collections;
public class Translation: object {
    private static string locale = "";
    /// <summary>
    /// 显示的文字
    /// </summary>
    public static string Locale {
        get {
            return locale;
        }
        set {
            if (locale == value) {
                return;
            }
            string newLanguage = Rollback(value);
            if (locale == newLanguage) {
                return;
            }
            locale = newLanguage;
            LangageChanged.Invoke();
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
    private static string Rollback(string langage) {
        if (langage == null) {
            return "";
        }
        if (langage == "") {
            return "";
        }
        if (DirAccess.DirExistsAbsolute(GetDir(langage))) {
            return langage;
        }
        // 不存在该语言文件，回滚
        // zh_HK -> zh
        langage = langage.Split("_")[0];
        if (DirAccess.DirExistsAbsolute(GetDir(langage))) {
            return langage;
        }
        // 回滚失败，不翻译
        return "";
    }
    public static string GetDir(string langage) {
        return OS.GetUserDataDir() + "/localization/" + langage + "/";
    }
    /// <summary>
    /// 翻译
    /// </summary>
    /// <param name="source">源字符串</param>
    /// <param name="content">上下文，实际上是翻译文件路径的一部分</param>
    /// <returns>翻译结果</returns>
    public static string Translate(string source, string content = "core", string language = "") {
        if (language == "") {
            language = Locale;
        }
        language = Rollback(language);
        if (language == "") {
            return source;
        }
        if (source == "") {
            return source;
        }
        string fileName = GetDir(language) + content + ".json";
        // 已经加载过该文件
        if (loaded.ContainsKey(fileName)) {
            if (loaded[fileName].ContainsKey(source)) {
                return loaded[fileName][source];
            }
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
        return source;
    }
}
