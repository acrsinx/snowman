"""
生成剧情文件
"""
import os
import re
import json
import time
from enum import Enum

import main
import formatCode

class PlotCodeType(Enum):
    """
    剧情代码的类型
    """
    intType = 1
    floatType = 2
    codeType = 3
    strType = 4

    def __str__(self) -> str:
        if self == PlotCodeType.intType:
            return "int"
        elif self == PlotCodeType.floatType:
            return "float"
        elif self == PlotCodeType.codeType:
            return "code"
        elif self == PlotCodeType.strType:
            return "str"
        else:
            raise Exception("出现奇怪的类型")

    def __repr__(self) -> str:
        return str(self)

def isIntType(string: str) -> bool:
    try:
        int(string)
        return True
    except ValueError:
        return False

def isFloatType(string: str) -> bool:
    try:
        float(string)
        return True
    except ValueError:
        return False


I: PlotCodeType = PlotCodeType.intType
F: PlotCodeType = PlotCodeType.floatType
C: PlotCodeType = PlotCodeType.codeType
S: PlotCodeType = PlotCodeType.strType

valueNum: dict[str, list[list[PlotCodeType]]] = {
    "CameraAnimation": [[I, C, C]],
    "LoadCharacter": [[S, S, F, F, F]],
    "SetCharacterTarget": [[S, F, F, F]],
    "SetCharacterPosition": [[S, F, F, F]],
    "PlayAnimation": [[S, S]],
    "PauseAnimation": [[S]],
    "LookAtCharacter": [[S, F, F]],
    "SetCameraPosition": [[]],
    "SetCameraPositionAt": [[F, F, F]],
    "SetCameraRotation": [[F, F]],
    "SetTaskName": [[S]],
    "Goto": [[I]],
    "AddTrigger": [[S, C]],
    "AddTarget": [[S, F, C]],
    "Jump": [[S]],
    "SetScene": [[S]],
    "EnterName": [[]],
    "ShowChooses": [[S, S, S]],
    "Exit": [[]]
}
"""
剧情脚本中每个指令的参数
"""

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

def split_line(code: str, markdown_file: str) -> list[str]:
    """
    分行
    """
    tab_level: int = 0
    last_index: int = 0
    ret: list[str] = []
    for i in range(len(code)):
        if code[i] == ";" and tab_level == 0:
            ret.append(code[last_index:i])
            last_index = i + 1
            continue
        if code[i] == "{":
            tab_level += 1
            continue
        if code[i] == "}":
            tab_level -= 1
            if tab_level < 0:
                os.utime(markdown_file, (time.time(), time.time()))
                raise Exception("括号不合理", code)
            continue
    if last_index < len(code):
        ret.append(code[last_index:])
    return ret

def split_token(code: str, markdown_file: str) -> list[str]:
    """
    分词元
    """
    ret: list[str] = []
    tab_level: int = 0
    last_index: int = 0
    for i in range(len(code)):
        if code[i] == " " and tab_level == 0:
            ret.append(code[last_index:i])
            last_index = i + 1
            continue
        if code[i] == "{":
            if tab_level == 0:
                ret.append(code[last_index:i])
                last_index = i
            tab_level += 1
            continue
        if code[i] == "}":
            tab_level -= 1
            if tab_level < 0:
                os.utime(markdown_file, (time.time(), time.time()))
                raise Exception("括号不合理", code)
            continue
    if last_index < len(code):
        ret.append(code[last_index:])
    return ret

def unwrap(code: str, markdown_file: str) -> str:
    """
    解开两侧的大括号
    """
    if code[0] == "{" and code[-1] == "}":
        return unwrap(code[1:-1], markdown_file)
    if code[0] == "{" and code[-1] != "}":
        os.utime(markdown_file, (time.time(), time.time()))
        raise Exception("未闭合的括号", code)
    return code

def check_script(code: str, markdown_file: str) -> None:
    """
    检查剧情脚本
    """
    code: str = unwrap(code, markdown_file)
    lines: list[str] = split_line(code, markdown_file)
    for line in lines:
        check_line(line, markdown_file)

def check_line(line: str, markdown_file: str) -> None:
    """
    检查剧情脚本
    """
    tokens: list[str] = split_token(line, markdown_file)
    if tokens[0] not in valueNum:
        os.utime(markdown_file, (time.time(), time.time()))
        raise Exception("未知指令："+tokens[0])
    parameterLists: list[list[PlotCodeType]] = valueNum[tokens[0]]
    for parameters in parameterLists:
        if len(parameters) + 1 != len(tokens):
            continue
        flag: bool = True
        for i in range(len(parameters)):
            if parameters[i] == PlotCodeType.intType and not isIntType(tokens[i+1]):
                flag = False
                continue
            if parameters[i] == PlotCodeType.floatType and not isFloatType(tokens[i+1]):
                flag = False
                continue
            if parameters[i] == PlotCodeType.codeType:
                check_script(tokens[i+1], markdown_file)
                continue
        if flag:
            break
    else:
        os.utime(markdown_file, (time.time(), time.time()))
        raise Exception("参数错误", tokens, "应为", parameterLists)

def simplify_script(code: str, markdown_file: str) -> str:
    """
    简化剧情脚本
    将无用符号换为空格
    以减小生成的json文件大小
    """
    # 将换行符、逗号、括号、空格、制表符换为空格
    simpleCode: str = re.sub(r'[\n,() \t]+', " ", code)
    # 去除首尾空格
    simpleCode = re.sub(r'^ | +$|', "", simpleCode)
    # 去除与";"相连的空格
    simpleCode = re.sub(r';+ | +;+ | +;', ";", simpleCode)
    # 去除与"{"相连的空格
    simpleCode = re.sub(r'{+ | +{+ | +{', "{", simpleCode)
    # 去除与"}"相连的空格
    simpleCode = re.sub(r'}+ | +}+ | +}', "}", simpleCode)
    check_script(simpleCode, markdown_file)
    return simpleCode

def make_json(is_release: bool = False) -> None:
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
        make_json_file(markdown_file, this_plot_dir, is_release)

def make_json_file(markdown_file: str, this_plot_dir: str, is_release: bool = False) -> None:
    """
    从一个 markdown 文件中生成 json 文件
    """
    with open(markdown_file, "r", encoding='utf-8') as f:
        data: str = f.read()
        tokens: list[str] = decode_md_plot(data)
        i: int = 0
        while i < len(tokens):
            if tokens[i] != "file":
                break
            fileName: str = this_plot_dir + tokens[i + 1]
            if os.path.exists(fileName):
                # 比较生成文件时间，如果生成文件时间比原文件晚，比Python脚本晚，则跳过，这样可以避免重复生成
                if formatCode.check_time(markdown_file, fileName) and not is_release:
                    i += 1
                    while tokens[i] != "file":
                        i += 1
                        if i >= len(tokens):
                            break
                    continue
            print("生成json文件: ", fileName)
            with open(fileName, "w", encoding='utf-8', newline="\n") as file_output:
                i += 2
                json_file_data = {}
                while i < len(tokens) and tokens[i] != "file":
                    caption_index: str = tokens[i]
                    captionType: str = tokens[i + 1]
                    if captionType == "caption":  # 对话
                        actorName: str = tokens[i + 2]
                        caption: str = tokens[i + 3]
                        startCode: str = simplify_script(tokens[i + 4], markdown_file)
                        json_line = {
                            caption_index: {
                                "actorName": actorName,
                                "caption": caption,
                                "type": captionType,
                                "startCode": startCode
                            }
                        }
                        i += 5
                    elif captionType == "shot": # 无对话镜头
                        startCode: str = simplify_script(tokens[i + 2], markdown_file)
                        json_line = {
                            caption_index: {
                                "type": captionType,
                                "startCode": startCode
                            }
                        }
                        i += 3
                    else:
                        os.utime(markdown_file, (time.time(), time.time()))
                        raise Exception("未知对话类型: ", captionType)
                    json_file_data.update(json_line)
                json.dump(json_file_data, file_output, ensure_ascii=False)
