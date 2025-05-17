"""
生成剧情文件
"""
import os
import re
import json

import main
import formatCode

def decode_md_plot(data: str) -> list[str]:
    """
    提取剧情脚本的内容
    """
    in_token: bool = False
    tokens: list[str] = []
    current_token: str = ""
    i: int = 0
    while i < len(data):
        if data[i] == "`":
            if in_token:
                tokens.append(current_token)
            in_token = not in_token
            current_token = ""
            if data[i:i+3] == "```":
                i += 3
                continue
            i += 1
            continue
        current_token += data[i]
        i += 1
    return tokens

def simplify_script(code: str) -> str:
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

def make_json() -> None:
    """
    生成剧情脚本
    """
    current_directory: str = main.check_current_directory()
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
            tokens: list[str] = decode_md_plot(data)
            i: int = 0
            while i < len(tokens):
                if tokens[i] != "file":
                    break
                fileName: str = this_plot_dir+tokens[i+1]
                if os.path.exists(fileName):
                    # 比较生成文件时间，如果生成文件时间比原文件晚，比Python脚本晚，则跳过，这样可以避免重复生成
                    if formatCode.check_time(markdown_file, fileName):
                        i += 1
                        while tokens[i] != "file":
                            i += 1
                            if i >= len(tokens):
                                break
                        continue
                print("生成json文件: ", fileName)
                with open(fileName, "w", encoding='utf-8') as file_output:
                    i += 2
                    json_file_data = {}
                    while i < len(tokens) and tokens[i] != "file":
                        caption_index: str = tokens[i]
                        actorName: str = tokens[i+1]
                        caption: str = tokens[i+2]
                        captionType: str = tokens[i+3]
                        startCode: str = simplify_script(tokens[i+4])
                        if captionType == "caption": # 对话
                            json_line = {
                                caption_index: {
                                    "actorName": actorName,
                                    "caption": caption,
                                    "type": captionType,
                                    "startCode": startCode,
                                    "endCode": simplify_script(tokens[i+5])
                                }
                            }
                            i += 6
                        elif captionType == "choose": # 选择
                            i += 5
                            texts = []
                            # 读取选项
                            while True:
                                # 选项文本
                                texts.append(tokens[i])
                                # 选项结果脚本
                                texts.append(simplify_script(tokens[i+1]))
                                i += 2
                                if tokens[i] == "endChoose":
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
