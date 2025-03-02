import json
import os

def make_translate(path: str, output: str) -> None:
    """
    生成翻译文件
    - path: 源文件路径
    - output: 输出文件路径
    """
    print(path)
    translation_json: dict[str, str] = {}
    with open(path, "r", encoding="utf-8") as file:
        lines: list[str] = file.readlines()
        for line in lines:
            tookens: list[str] = line.split("|")
            tookens = [tooken for tooken in tookens if tooken != "" and tooken != "\n"]
            if len(tookens) != 2: # 跳过不合规行
                continue
            if True in [tooken == "---" for tooken in tookens]: # 跳过无用行
                continue
            if tookens[0] == tookens[1]: # 提醒无需翻译
                print("无需翻译: " + tookens[0])
            if tookens[1] == "-": # 跳过无需翻译行
                continue
            translation_json[tookens[0]] = tookens[1]
    if not os.path.exists(os.path.dirname(output)):
        os.mkdir(os.path.dirname(output))
    with open(output, "w", encoding="utf-8") as file:
        json.dump(translation_json, file)

def make_translate_all() -> None:
    """
    生成所有翻译文件
    """
    for language in os.listdir("localization\\"):
        if not os.path.isdir("localization\\"+language):
            continue
        if language == "template":
            continue
        if language == "localization":
            continue
        for file in os.listdir("localization\\"+language+"\\"):
            filePath: str = "localization\\"+language+"\\"+file
            if os.path.isdir(filePath):
                for subFile in os.listdir(filePath):
                    if not subFile.endswith(".md"):
                        continue
                    make_translate(filePath+"\\"+subFile, "localization\\localization\\"+language+"\\"+file+"\\"+subFile.replace(".md", ".json"))
            if not file.endswith(".md"):
                continue
            make_translate(filePath, "localization\\localization\\"+language+"\\"+file.replace(".md", ".json"))

def plot_json_path_to_template_path(path: str) -> str:
    """
    将plotJson路径转换为本地化模板路径
    """
    if not os.path.exists("localization\\template\\plot\\"):
        os.mkdir("localization\\template\\plot\\")
    return "localization\\template\\plot\\" + path.replace("plotJson\\", "").replace("\\", "_").replace(".json", ".md")

def create_localization_template(path: str) -> None:
    """
    创建翻译模板
    """
    output: str = plot_json_path_to_template_path(path)
    print(output)
    toTranslate: dict[str, str] = {}
    with open(path, "r", encoding="utf-8") as file:
        plotJson: dict[str, dict] = json.load(file)
        for i in range(len(plotJson)):
            toTranslate[plotJson[str(i)]["caption"]] = ""
            for n in range(3):
                if str(n) in plotJson[str(i)].keys():
                    toTranslate[next(iter(plotJson[str(i)][str(n)]))] = ""
    with open(output, "w", encoding="utf-8") as file:
        file.write("|||\n|---|---|\n")
        for key in toTranslate:
            file.write("|" + key + "||\n")

def create_localization_template_all() -> None:
    """
    创建所有翻译模板
    """
    for file in os.listdir("plotJson\\"):
        subPath: str = "plotJson\\" + file
        if not os.path.isdir(subPath):
            continue
        for subFile in os.listdir(subPath):
            if not subFile.endswith(".json"):
                continue
            create_localization_template(subPath + "\\" + subFile)

if __name__ == '__main__':
    create_localization_template_all()
    make_translate_all()
