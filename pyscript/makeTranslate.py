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
        if language == "localization":
            continue
        for file in os.listdir("localization\\"+language+"\\"):
            if not file.endswith(".md"):
                continue
            make_translate("localization\\"+language+"\\"+file, "localization\\localization\\"+language+"\\"+file.replace(".md", ".json"))

if __name__ == '__main__':
    make_translate_all()
