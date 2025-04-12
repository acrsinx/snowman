"""
生成翻译文件
"""
import json
import os
import re

import formatCode

def read_translation_file(path: str) -> list[tuple[str, str]]:
    """
    读取翻译markdown文件
    - path: 文件路径
    """
    ret: list[tuple[str, str]] = []
    with open(path, "r", encoding="utf-8") as file:
        lines: list[str] = file.readlines()
        for line in lines:
            tokens: list[str] = line.split("|")
            # 去除首尾
            tokens: list[str] = tokens[1:-1]
            if len(tokens) != 2: # 跳过不合规行
                continue
            if tokens[0] == "": # 跳过空行
                continue
            if tokens[0].isspace(): # 跳过空行
                continue
            if True in [token == "---" for token in tokens]: # 跳过无用行
                continue
            ret.append((tokens[0], tokens[1]))
    return ret

def make_translate(path: str, output: str) -> None:
    """
    生成翻译文件
    - path: 源文件路径
    - output: 输出文件路径
    """
    # 本地化模板路径
    template_path: str = localization_path_to_template_path(path)
    # 未被修改则跳过
    if formatCode.check_time(path, output) and formatCode.check_time(template_path, output):
        return
    print("生成翻译文件: ", path)
    translation_json: dict[str, str] = {}
    # 读取翻译文件
    translation_file: list[tuple[str, str]] = read_translation_file(path)
    # 读取本地化模板
    template_file: list[tuple[str, str]] = read_translation_file(template_path)
    for i in range(len(translation_file)):
        if i >= len(template_file): # 加入了不需要翻译的语段
            print("无需加入", translation_file[i][0])
            break
        tokens: tuple[str, str] = translation_file[i]
        if tokens[0] != template_file[i][0]: # 提醒翻译文件与模板文件不一致
            print("不一致: " + tokens[0] + " != " + template_file[i][0])
        if tokens[0] == tokens[1]: # 提醒无需翻译
            print("无需翻译: " + tokens[0])
        if tokens[1].isspace() or tokens[1] == "": # 提醒未翻译
            print("未翻译: " + tokens[0])
        if tokens[1] == "-": # 跳过无需翻译行
            continue
        translation_json[tokens[0]] = tokens[1]
    if len(translation_file) < len(template_file): # 缺少翻译
        print("缺少翻译")
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
    return "localization\\template\\plot\\" + path.replace("plotJson\\", "").replace("\\", "_").replace(".json", ".md")

def localization_path_to_template_path(path: str) -> str:
    """
    将本地化markdown路径转换为本地化模板路径
    """
    # 将语言段换成"template"即可
    return re.sub(r"localization\\(.*?)\\", r"localization\\template\\", path)

def create_localization_template(path: str) -> None:
    """
    创建翻译模板
    """
    output: str = plot_json_path_to_template_path(path)
    # 未被修改则跳过
    if formatCode.check_time(path, output):
        return
    print("创建翻译模板: ", output)
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
