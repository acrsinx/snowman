import os
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

def is_operator(operator: str) -> bool:
    """
    判断是否是符号
    """
    return operator in ["=", "==", "+", "-", "*", "/", "%", "++", "--", "&&", "||", "!", ">", "<", ">=", "<=", "==", "!=", "&", "|", "^", "~", "<<", ">>", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>="]

def split_word(data: str) -> list[tuple[str, bool]]:
    """
    分词

    针对"{}"的编程语言

    返回
    - list[str, bool] str: 词元, bool: 是否是注释
    """
    words: list[str, bool] = []
    word: str = ""
    is_note: bool = False
    is_string: bool = False
    for i in range(len(data)):
        if is_note:
            if data[i] == "\n":
                is_note = False
                if len(word) > 0:
                    words.append((word, True))
                    word = ""
                continue
            word += data[i]
            continue
        if data[i] in [" ", "\n", "\t", "\r", "\v"]:
            if len(word) > 0:
                words.append((word, False))
                word = ""
            continue
        if data[i] in [",", ";", "{", "}", "(", ")", "[", "]", "?", ":"]:
            if len(word) > 0:
                words.append((word, False))
                word = ""
            words.append((data[i], False))
            continue
        if data[i] == "\"":
            if not is_string:
                if len(word) > 0:
                    words.append((word, False))
                    word = ""
            is_string = not is_string
        if data[i] == "/" and data[i+1] == "/":
            if len(word) > 0:
                words.append((word, False))
                word = ""
            is_note = True
            word += data[i]
            continue
        word += data[i]
    if len(word) > 0:
        words.append((word, False))
    return words

def output(path: str, words: list[tuple[str, bool]]):
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
            if words[i][0] == ")": # for循环结束
                in_for = False
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
            if (words[i][0] == ";" or words[i][1] or (words[i][0] == "," and in_array and tab_level_in_array == 1)) and not in_for: # 如果是";"，或注释，或数组中特定级别的"," 且不是for循环的()中，则加换行
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
            if words[i+1][0] == "(" and words[i][0] not in ["for", "elif", "switch", "if", "while"] and not words[i][0].endswith("="): # 如果是(之前且不是关键词，则不加空格
                continue
            if words[i][0] == "{": # 如果是{，则增加缩进
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
    with open(path, "r", encoding="utf-8") as f:
        data: str = f.read()
        # 分词
        words: list[tuple[str, bool]] = split_word(data)
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
    current_directory: str = main.check_current_directory()
    ignore_list, _ = read_ignore(current_directory + "\\.gitignore")
    dir_list: list[str] = os.listdir(current_directory)
    # 将斜杠替换为反斜杠
    for i in range(len(ignore_list)):
        ignore_list[i] = ignore_list[i].replace("/", "\\")
    arrange_code(current_directory, current_directory, dir_list, ignore_list+[".git\\", "export\\"])
