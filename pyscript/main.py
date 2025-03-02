import os
import json
import re

import arrangeCode
import makeTranslate

def decode_mdplot(data: str) -> list[str]:
    in_tooken: bool = False
    tookens: list[str] = []
    current_tooken: str = ""
    i: int = 0
    while i < len(data):
        if data[i] == "`":
            if in_tooken:
                tookens.append(current_tooken)
            in_tooken = not in_tooken
            current_tooken = ""
            if data[i:i+3] == "```":
                i += 3
                continue
            i += 1
            continue
        current_tooken += data[i]
        i += 1
    return tookens

def simplify_script (code: str) -> str:
    """
    简化剧情脚本
    将无用符号换为空格
    以减小生成的json文件大小
    """
    # 将换行符、逗号、括号、空格换为空格
    simpleCode: str = re.sub(r'[\n,() ]+', " ", code)
    # 去除首尾空格
    simpleCode = re.sub(r'^ | +$|', "", simpleCode)
    # 去除与";"相连的空格
    simpleCode = re.sub(r';+ | +;+ | +;', ";", simpleCode)
    return simpleCode

def check_current_directory() -> str:
    """
    设置正确的工作目录
    """
    os.chdir(os.path.dirname(os.path.dirname(__file__)))
    return os.getcwd()

def make_json() -> None:
    """
    生成剧情脚本
    """
    current_directory: str = check_current_directory()
    # 检查文件夹的各个文件
    plot_dir: str = current_directory+"\\plot\\"
    plot_json_dir: str = current_directory+"\\plotJson\\"
    files: list[str] = os.listdir(plot_dir)
    files = [file for file in files if file.endswith(".md")]
    os.makedirs(plot_json_dir, exist_ok=True)

    for file in files:
        this_plot_dir: str = plot_json_dir+file[:-3]+"\\"
        os.makedirs(this_plot_dir, exist_ok=True)
        markdown_file: str = plot_dir+file
        with open(markdown_file, "r", encoding='utf-8') as f:
            data: str = f.read()
            tookens: list[str] = decode_mdplot(data)
            i: int = 0
            while i < len(tookens):
                if tookens[i] != "file":
                    break
                fileName: str = this_plot_dir+tookens[i+1]
                if os.path.exists(fileName):
                    # 比较生成文件时间，如果生成文件时间比原文件晚，比Python脚本晚，则跳过，这样可以避免重复生成
                    if arrangeCode.check_time(markdown_file, fileName):
                        i += 1
                        while tookens[i] != "file":
                            i += 1
                            if i >= len(tookens):
                                break
                        continue
                print("生成json文件: ", fileName)
                with open(fileName, "w", encoding='utf-8') as file_output:
                    i += 2
                    json_file_data = {}
                    while i < len(tookens) and tookens[i] != "file":
                        caption_index: str = tookens[i]
                        actorName: str = tookens[i+1]
                        caption: str = tookens[i+2]
                        captionType: str = tookens[i+3]
                        startCode: str = simplify_script(tookens[i+4])
                        if captionType == "caption": # 对话
                            json_line = {
                                caption_index: {
                                    "actorName": actorName,
                                    "caption": caption,
                                    "type": captionType,
                                    "startCode": startCode,
                                    "endCode": simplify_script(tookens[i+5])
                                }
                            }
                            i += 6
                        elif captionType == "choose": # 选择
                            i = i + 5
                            texts = []
                            # 读取选项
                            while True:
                                # 选项文本
                                texts.append(tookens[i])
                                # 选项结果脚本
                                texts.append(simplify_script(tookens[i+1]))
                                i += 2
                                if tookens[i] == "endChoose":
                                    i += 1
                                    break
                            texts_json: dict = {}
                            for j in range(len(texts) // 2):
                                texts_json.update({
                                    j: {
                                        texts[j*2]: texts[j*2+1]
                                    }
                                })
                            a_json: dict = {
                                    "actorName": actorName,
                                    "caption": caption,
                                    "type": captionType,
                                    "startCode": startCode,
                                }
                            a_json.update(texts_json)
                            json_line = {
                                caption_index: a_json
                            }
                        else:
                            print("未知对话类型: ", captionType)
                            break
                        json_file_data.update(json_line)
                    json.dump(json_file_data, file_output, ensure_ascii=False)

need_dirs: list[str] = ["export\\export\\"]

def make_dir() -> None:
    for dir in need_dirs:
        if not os.path.exists(dir):
            os.makedirs(dir)
            print("创建目录: ", dir)

if __name__ == '__main__':
    make_dir()
    arrangeCode.arrange_whole_project()
    make_json()
    makeTranslate.create_localization_template_all()
    makeTranslate.make_translate_all()
