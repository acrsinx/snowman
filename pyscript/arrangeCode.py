from enum import Enum
import os
import shutil
import main

def read_ignore(path: str) -> tuple[list[str], list[str]]:
    """
    读取忽略文件

    返回
    - 忽略的文件夹
    - 忽略的文件后缀
    """
    with open(path, "r", encoding="utf-8") as f:
        # 分行
        lines: list[str] = f.read().splitlines()
        suffix: list[str] = []
        for i in range(len(lines)):
            if len(lines) <= i: # 防止越界
                break
            if lines[i].startswith("#") or len(lines[i].strip()) == 0: # 忽略注释和空行
                lines.pop(i)
            lines[i] = lines[i].strip() # 去除空格
            if lines[i].startswith("*."): # 如果是忽略的文件
                suffix.append(lines[i][2:])
                lines.pop(i)
        return lines, suffix

"""
符号列表
从短到长
"""
operator_list: list[str] = ["=", "+", "-", "*", "/", "%", "!", ">", "<", "&", "|", "^", "~", "==", "++", "--", "&&", "||", ">=", "<=", "==", "!=", "<<", ">>", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", "??="]

def is_operator(operator: str) -> bool:
    """
    判断是否是符号
    """
    return operator in operator_list

class NoteType(Enum):
    """
    类型
    - 0 普通
    - 1 注释
    - 2 行末注释
    """
    NORMAL = 0
    NOTE = 1
    NOTE_END = 2

def split_word(data: str) -> list[tuple[str, NoteType]]:
    """
    分词

    针对"{}"的编程语言

    返回
    - list[str, NoteType] str: 词元 NoteType: 注释类型
    """
    words: list[str, NoteType] = []
    word: str = ""
    is_note: NoteType = NoteType.NORMAL
    is_string: bool = False
    is_char: bool = False
    operator: int = -1
    operator_length = -1
    operator_start: int = -1
    for i in range(len(data)):
        # 算符
        if i - operator_start < operator_length:
            continue
        if operator_start != -1 and i - operator_start >= operator_length:
            print(operator_list[operator])
            words.append((operator_list[operator], NoteType.NORMAL))
            operator_start = -1
            operator_length = -1
            operator = -1
            word = ""
        # 注释
        if is_note != NoteType.NORMAL:
            if data[i] == "\n":
                if len(word) > 0:
                    words.append((word, is_note))
                    word = ""
                is_note = NoteType.NORMAL
                continue
            word += data[i]
            continue
        # 字符串
        if data[i] == "\"":
            if not is_string:
                if len(word) > 0:
                    words.append((word, NoteType.NORMAL))
                    word = ""
            is_string = not is_string
        if is_string:
            word += data[i]
            continue
        # 字符
        if data[i] == "'":
            if not is_char:
                if len(word) > 0:
                    words.append((word, NoteType.NORMAL))
                    word = ""
            is_char = not is_char
        if is_char:
            word += data[i]
            continue
        # 注释
        if data[i] == "/" and data[i+1] == "/":
            if len(word) > 0:
                words.append((word, NoteType.NORMAL))
                word = ""
            # 是否是行末注释
            is_note = NoteType.NOTE
            j: int = i
            while j >= 0 and data[j] != "\n":
                j -= 1
                if not data[j].isspace(): # 如果不是空格，则是行末注释
                    is_note = NoteType.NOTE_END
                    break
            word += data[i]
            continue
        # 空白
        if data[i].isspace():
            if len(word) > 0:
                words.append((word, NoteType.NORMAL))
                word = ""
            continue
        if data[i:].startswith(tuple(operator_list)):
            if len(word) > 0:
                words.append((word, NoteType.NORMAL))
                word = ""
            # 匹配是哪一个算符
            for j in range(len(operator_list)-1, -1, -1):
                if data[i:].startswith(operator_list[j]):
                    operator = j
                    operator_start = i
                    operator_length = len(operator_list[j])
                    break
            continue
        # 特殊符号
        if data[i] in [",", ";", "{", "}", "(", ")", "[", "]", "?", ":"]:
            if len(word) > 0:
                words.append((word, NoteType.NORMAL))
                word = ""
            words.append((data[i], NoteType.NORMAL))
            continue
        word += data[i]
    if len(word) > 0:
        words.append((word, NoteType.NORMAL))
    # [print(i[0], "\t", i[1]) for i in words]
    return words

def output(path: str, words: list[tuple[str, NoteType]]):
    """
    输出
    """
    with open(path, "w", encoding="utf-8") as f:
        tab_level: int = 0
        # 是否在for循环的()中
        in_for: bool = False
        # 是否在数组中，内无分号的就是数组，数组中相对缩进为1的展开，其余不展开
        in_array: bool = False
        # 数组结束的位置
        array_end: int = -1
        # 相对缩进
        tab_level_in_array: int = 0
        for i in range(len(words)):
            f.write(words[i][0])
            if i+1 >= len(words): # 最后一行
                f.write("\n")
                return
            if words[i][0] == "for": # for循环
                in_for = True
            if words[i][0] == ")": # for循环结束或数组缩进减少
                in_for = False
                if in_array:
                    tab_level_in_array -= 1
                    if tab_level_in_array == 0:
                        tab_level -= 1
            if words[i+1][0] == "}": # 如果下一个词是"}"且不在数组中，则减少缩进
                if in_array:
                    tab_level_in_array -= 1
                    if tab_level_in_array != 0:
                        continue
                tab_level -= 1
                if words[i][0] != ";": # 如果当前词不是";"，则补充换行
                    f.write("\n")
                    f.write(" " * 4 * tab_level)
                    continue
            if words[i][0] == "}": # "}"后补充换行
                if in_array:
                    if i >= array_end:
                        in_array = False
                        tab_level_in_array = 0
                    continue
                if (words[i+1][0] not in ["else", "elif"]) and (not words[i+1][0] == ";"):
                    f.write("\n")
                    f.write(" " * 4 * tab_level)
                    continue
            if words[i][0] == "(": # 数组内的"("增大缩进
                if in_array:
                    tab_level_in_array += 1
                    if tab_level_in_array == 1:
                        f.write("\n")
                        tab_level += 1
                        f.write(" " * 4 * tab_level)
            if (words[i][0] == ";" or words[i][1] != NoteType.NORMAL or (words[i][0] == "," and in_array and tab_level_in_array == 1)) and not in_for: # 如果是";"，或注释，或数组中特定级别的"," 且不是for循环的()中，则加换行
                f.write("\n")
                f.write(" " * 4 * tab_level)
                continue
            if words[i][0] == "\"" and words[i+1][0] == "\"": # 如果是""，则不加空格
                continue
            if words[i][0] == "'" and words[i+1][0] == "'": # 如果是''，则不加空格
                continue
            if words[i][0] == ":" and words[i+1][0].startswith("\""): # 如果是:"，则不加空格
                continue
            if words[i][0] in [")", "]"] and words[i+1][0].startswith("."): # 如果是")."或"]."，则不加空格
                continue
            if words[i][0] in ["(", "[", "\"", "'", "?"]: # 如果是(或[或"或'或?之后，则不加空格
                continue
            if words[i+1][0] in ["[", "]", ",", ":", ";", "\"", "'", ")", "?"] and not is_operator(words[i][0]): # 如果是[或]或,或:或;或"或'或)或?之前，且不是算符，则不加空格
                continue
            if words[i+1][0] == "(" and words[i][0] not in [",", "for", "foreach", "elif", "switch", "if", "while"] and not words[i][0].endswith("="): # 如果是(之前且不是关键词，则不加空格
                continue
            if words[i][0] == "{": # 如果是{，则增加缩进
                if words[i+1][1] == NoteType.NOTE_END: # 如果是行末注释，则不立即换行
                    tab_level += 1
                    f.write(" ")
                    continue
                if in_array:
                    tab_level_in_array += 1
                    if tab_level_in_array == 1:
                        f.write("\n")
                        tab_level += 1
                        f.write(" " * 4 * tab_level)
                    continue
                j: int = i
                tab_level_in: int = 0
                while True: # 获取数组信息
                    j += 1
                    if words[j][0] == "}": # 下一个词是同一级的"}"，而中间没有";"，则说明是数组
                        if tab_level_in != 0:
                            tab_level_in -= 1
                            continue
                        in_array = True
                        array_end = j
                        break
                    if words[j][0] == "{": # 下一个词是"{"
                        tab_level_in += 1
                        continue
                    if words[j][0] == ";":
                        break
                    if j >= len(words):
                        print("数组越界，代码格式定有错误", path)
                        break
                if in_array: # 如果是数组
                    tab_level_in_array = 1
                f.write("\n")
                tab_level += 1
                f.write(" " * 4 * tab_level)
                continue
            f.write(" ")

def arrange(path: str):
    """
    整理代码
    """
    print(path)
    # 留一个副本
    copy_path: str = "copy\\" + os.path.relpath(path, os.getcwd()) + ".copy"
    if not os.path.exists(os.path.dirname(copy_path)):
        os.makedirs(os.path.dirname(copy_path))
    shutil.copy(path, copy_path)
    with open(path, "r", encoding="utf-8") as f:
        data: str = f.read()
        # 分词
        words: list[tuple[str, NoteType]] = split_word(data)
        # 输出
        if path.endswith(".cs") or path.endswith(".gdshader"): # {}类编程语言，如C#、gdshader
            output(path, words)

def arrange_code(base_dir: str, current_dir: str, dir_list: list[str], ignore_list: list[str]):
    """
    整理代码
    """
    # 文件后缀白名单
    allow_suffix_list: list[str] = ["md", "cs", "gdshader"]
    for dir in dir_list:
        real_dir: str = os.path.join(current_dir, dir)
        relpath: str = os.path.relpath(real_dir, base_dir) + "\\"
        if any([i in relpath for i in ignore_list]): # 忽略文件夹
            continue
        if os.path.isdir(real_dir): # 如果是文件夹
            arrange_code(base_dir, real_dir, os.listdir(real_dir), ignore_list)
            continue
        if real_dir.split(".")[-1] not in allow_suffix_list: # 如果不是白名单文件
            continue
        arrange(real_dir)

if __name__ == "__main__":
    arrange("script\Camera.cs")
    exit()
    current_directory: str = main.check_current_directory()
    ignore_list, _ = read_ignore(current_directory + "\\.gitignore")
    dir_list: list[str] = os.listdir(current_directory)
    # 将斜杠替换为反斜杠
    for i in range(len(ignore_list)):
        ignore_list[i] = ignore_list[i].replace("/", "\\")
    arrange_code(current_directory, current_directory, dir_list, ignore_list+[".git\\", "export\\"])
