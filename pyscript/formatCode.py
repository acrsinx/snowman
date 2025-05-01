"""
格式化代码
"""
from enum import Enum
import os
import shutil
import time
import main

pyscript_path: str = os.path.dirname(os.path.abspath(__file__))
this_time: float = max([os.path.getmtime(os.path.join(pyscript_path, file)) for file in os.listdir(pyscript_path)])
"""
上次修改时间
- 取pyscript文件夹下所有文件的最晚修改时间
- 用于判断是否需要重新生成文件
"""
operator_list: list[str] = ["=", "+", "-", "*", "/", "%", "!", ">", "<", "&", "|", "^", "~", "==", "++", "--", "&&",
                            "||", ">=", "<=", "==", "!=", "<<", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "=>",
                            "??", "::", "<<=", ">>=", "??="]
"""
符号列表
从短到长
"""

operator_tuple: tuple = tuple(operator_list)

operator_before_left_parenthesis: tuple = (",", ";", "{", "for", "foreach", "elif", "switch", "if", "while", "return")
"""
后面加左括号要空格的
"""

operator_after_right_parenthesis: tuple = (".", ",", ";", "(", ")", "]", "[")
"""
前面加右括号不空格的
"""

operator_before_left_square_bracket: tuple = (";", "}")
"""
后面加左方括号要空开的
"""

operator_after_right_square_bracket: tuple = (".", ";")
"""
前面加右方括号不空格的
"""

operator_combine_right: tuple = ("?", "!", ".")
"""
右结合的符号
"""

operator_combine_left: tuple = ("++", "--", ".", "?.")
"""
左结合的符号
"""

operator_before_minus_sign: tuple = ("return", "=", "==", "(", "[", ",")
"""
这些词之后的“-”一定是负号而非减号
"""

def check_time(source_path: str, target_path: str) -> bool:
    """
    检查时间

    返回
    - bool 是否可以跳过生成文件
    """
    return os.path.getmtime(source_path) < os.path.getmtime(target_path) and this_time < os.path.getmtime(target_path)

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
            if len(lines) <= i:  # 防止越界
                break
            if lines[i].startswith("#") or len(lines[i].strip()) == 0:  # 忽略注释和空行
                lines.pop(i)
            lines[i] = lines[i].strip()  # 去除空格
            if lines[i].startswith("*."):  # 如果是忽略的文件
                suffix.append(lines[i][2:])
                lines.pop(i)
        return lines, suffix


def is_operator(operator: str) -> bool:
    """
    判断是否是符号
    """
    return operator in operator_list


class NoteType(Enum):
    """
    注释类型
    - 0 普通
    - 1 注释
    - 2 行末注释
    - 注意C#等语言中以#开头的不是注释，但当作注释处理，以求方便
    """
    NORMAL = 0
    NOTE = 1
    NOTE_END = 2

def package(token: str) -> tuple[str, NoteType]:
    """
    将字符串封装成特定的元组
    """
    return token, NoteType.NORMAL

def split_word(data: str) -> list[tuple[str, NoteType]]:
    """
    分词

    针对"{}"的编程语言

    返回
    - list[str, NoteType] str: 词元 NoteType: 注释类型
    """
    words: list[tuple[str, NoteType]] = []
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
            words.append(package(operator_list[operator]))
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
                    words.append(package(word))
                    word = ""
            is_string = not is_string
        if is_string:
            word += data[i]
            continue
        # 字符
        if data[i] == "'":
            if not is_char:
                if len(word) > 0:
                    words.append(package(word))
                    word = ""
            is_char = not is_char
        if is_char:
            word += data[i]
            continue
        # 注释
        if (data[i] == "/" and data[i + 1] == "/") or (data[i] == "#" and data[i + 1] == " "):
            if len(word) > 0:
                words.append(package(word))
                word = ""
            # 是否是行末注释
            is_note = NoteType.NOTE
            j: int = i
            while j >= 0 and data[j] != "\n":
                j -= 1
                if not data[j].isspace():  # 如果不是空格，则是行末注释
                    is_note = NoteType.NOTE_END
                    break
            word += data[i]
            continue
        # 空白
        if data[i].isspace():
            if len(word) > 0:
                words.append(package(word))
                word = ""
            continue
        # 算符
        if data[i:].startswith(tuple(operator_list)):
            if len(word) > 0:
                words.append(package(word))
                word = ""
            # 匹配是哪一个算符
            for j in range(len(operator_list) - 1, -1, -1):
                if data[i:].startswith(operator_list[j]):
                    operator = j
                    operator_start = i
                    operator_length = len(operator_list[j])
                    break
            continue
        # 特殊符号
        if data[i] in [",", ";", "{", "}", "(", ")", "[", "]", "?", ":"]:
            if len(word) > 0:
                words.append(package(word))
                word = ""
            words.append(package(data[i]))
            continue
        word += data[i]
    if len(word) > 0:
        words.append(package(word))
    return words

def find_and_combine_ternary_operator(words: list[tuple[str, NoteType]]) -> None:
    """
    找出三元运算符?:中的:位置，并合并
    """
    # 三元运算符?:中的:索引
    have_question: bool = False
    question_indexes: list[int] = []
    colon_indexes: list[int] = []
    i: int = 0
    while i < len(words):
        if words[i][0] == "?":
            have_question = True
            question_indexes.append(i)
            i += 1
            continue
        if words[i][0] in ["{", "}", ";"]:  # 三元运算符?:中一定没有的符号
            have_question = False
            i += 1
            continue
        if words[i][0] == ":" and have_question:  # 三元运算符?:中的:索引
            have_question = False
            colon_indexes.append(i)
            # 合并
            for to_merge in [question_indexes[-1], i - 2]:
                words[to_merge-1] = (words[to_merge-1][0] + words[to_merge][0] + words[to_merge+1][0], words[i - 1][1])
                words.pop(to_merge)
                words.pop(to_merge)
            i -= 4
        i += 1

def combine_colon(words: list[tuple[str, NoteType]]) -> None:
    """
    把冒号与前一个词合并
    """
    i: int = 1
    while i < len(words):
        if words[i][0] == "::":
            words[i - 1] = (words[i - 1][0] + "::" + words[i+1][0], words[i - 1][1])
            words.pop(i)
            words.pop(i)
            i += 1
            continue
        if words[i][0] != ":":
            i += 1
            continue
        words[i - 1] = (words[i - 1][0] + ":", words[i - 1][1])
        words.pop(i)
        i += 1

def combine_generic_angle_brackets(words: list[tuple[str, NoteType]]) -> None:
    """
    把泛型的尖括号合并
    """
    # 尖括号成对出现
    level: int = 0
    level_list: list[int] = []
    # 尖括号所在位置的索引
    indexes: list[tuple[int, bool]] = []
    for i in range(len(words)):
        level_list.append(level)
        if words[i][0] == "<":
            level += 1
            continue
        if words[i][0] == ">":
            level -= 1
            if level < 0:  # 一定不是泛型的尖括号
                level = 0
                continue
            indexes.append((i, False))
            j: int = i - 1
            while level_list[j] > level:  # 找到头
                j -= 1
            indexes.append((j, True))
            continue
        if words[i][0] in ["{", "}"] + operator_list:  # 泛型的两个尖括号中一定没有的符号
            level = 0
            continue
    indexes.sort(key=lambda x: x[0], reverse=True)
    for to_merge in indexes:
        if to_merge[1]:  # 左括号
            words[to_merge[0] - 1] = (words[to_merge[0] - 1][0] + "<" + words[to_merge[0] + 1][0], words[to_merge[0] - 1][1])
            words.pop(to_merge[0])
            words.pop(to_merge[0])
            continue
        words[to_merge[0] - 1] = (words[to_merge[0] - 1][0] + words[to_merge[0]][0], words[to_merge[0] - 1][1])
        words.pop(to_merge[0])

def combine_parentheses(words: list[tuple[str, NoteType]]) -> None:
    """
    合并小括号
    """
    i: int = 0
    while i < len(words):
        if words[i][1] != NoteType.NORMAL:
            i += 1
            continue
        if words[i][0].startswith('('):
            if not (words[i - 1][0].endswith(operator_before_left_parenthesis) or (words[i - 1][0].endswith(operator_tuple) and not words[i - 1][0].endswith(">"))):
                words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
                words.pop(i)
                i -= 1
        if words[i][0].endswith('('):
            words[i] = (words[i][0] + words[i + 1][0], words[i][1])
            words.pop(i + 1)
            i -= 1
        if words[i][0].endswith(')'):
            if not words[i + 1][0].startswith("}"):
                if words[i + 1][0].startswith(operator_after_right_parenthesis):
                    words[i] = (words[i][0] + words[i + 1][0], words[i][1])
                    words.pop(i + 1)
                    i -= 1
                else:
                    words[i] = (words[i][0] + " " + words[i + 1][0], words[i][1])
                    words.pop(i + 1)
                    i -= 1
        if words[i][0].startswith(')'):
            words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
            words.pop(i)
            i -= 1
        i += 1

def combine_square_brackets(words: list[tuple[str, NoteType]]) -> None:
    """
    合并方括号
    """
    i: int = 0
    while i < len(words):
        if words[i][0].startswith('['):
            if not (is_operator(words[i - 1][0]) or words[i - 1][0].endswith(operator_before_left_square_bracket)):
                words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
                words.pop(i)
                i -= 1
        if words[i][0].endswith('['):
            if not is_operator(words[i + 1][0]):
                words[i] = (words[i][0] + words[i + 1][0], words[i][1])
                words.pop(i + 1)
                i -= 1
        if words[i][0].startswith(']'):
            if not is_operator(words[i - 1][0]):
                words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
                words.pop(i)
                i -= 1
        if words[i][0].endswith(']'):
            if words[i + 1][0].startswith(operator_after_right_square_bracket):
                words[i] = (words[i][0] + words[i + 1][0], words[i][1])
                words.pop(i + 1)
                i -= 1
        i += 1

def combine_semicolon(words: list[tuple[str, NoteType]]) -> None:
    """
    把分号与前一个词合并
    """
    i: int = 0
    while i < len(words):
        if words[i][0].startswith(";"):
            words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
            words.pop(i)
        i += 1

def combine_comma(words: list[tuple[str, NoteType]]) -> None:
    """
    把逗号与前一个词合并
    """
    i: int = 0
    while i < len(words):
        if words[i][0].startswith(","):
            words[i - 1] = (words[i - 1][0] + words[i][0], words[i - 1][1])
            words.pop(i)
        i += 1

def combine_operator(words: list[tuple[str, NoteType]]) -> None:
    """
    合并运算符
    """
    i: int = 0
    while True:
        if words[i][0].endswith(operator_combine_right):
            words[i] = (words[i][0] + words[i+1][0], words[i][1])
            words.pop(i+1)
        if i >= len(words) - 1:
            break
        if words[i + 1][0].startswith(operator_combine_left):
            words[i] = (words[i][0] + words[i+1][0], words[i][1])
            words.pop(i+1)
        if words[i][0] == "-" and words[i-1][0].endswith(operator_before_minus_sign):
            words[i] = (words[i][0] + words[i+1][0], words[i][1])
            words.pop(i+1)
        if words[i][0].endswith("-") and words[i][0][:-1].endswith(operator_before_minus_sign):
            words[i] = (words[i][0] + words[i+1][0], words[i][1])
            words.pop(i+1)
        i += 1

def find_array_comma(words: list[tuple[str, NoteType]]) -> list[tuple[str, NoteType, bool]]:
    """
    找出数组中的逗号
    """
    # 大括号中没有";"的区域为数组
    output_list: list[tuple[str, NoteType, bool]] = []
    for token, note_type in words:
        output_list.append((token, note_type, False))
    for i in range(len(output_list)):
        if not output_list[i][0].endswith(","):
            continue
        flag: bool = True
        tab_level: int = 0
        for j in range(i + 1, len(output_list)):
            if output_list[j][0] == "};":
                tab_level -= 1
                if tab_level < 0:
                    break
                flag = False
                continue
            if output_list[j][0].endswith(";"):
                flag = False
                break
            if output_list[j][0].endswith("{"):
                tab_level += 1
                continue
            if output_list[j][0].startswith("}"):
                tab_level -= 1
                if tab_level < 0:
                    break
                continue
        tab_level = 0
        for j in range(i - 1, -1, -1):
            if output_list[j][0].endswith(";"):
                flag = False
                break
            if output_list[j][0].startswith("}"):
                tab_level += 1
                continue
            if output_list[j][0].endswith("{"):
                tab_level -= 1
                if tab_level < 0:
                    break
                continue
        output_list[i] = (output_list[i][0], output_list[i][1], flag)
    return output_list

def is_end_of_line(words: list[tuple[str, NoteType, bool]], i: int) -> bool:
    """
    判断是否是行尾
    """
    if words[i][0].endswith((";", "{")) and (words[i + 1][1] is not NoteType.NOTE_END):
        return True
    if words[i + 1][0].startswith("}"):
        return True
    if words[i][0].endswith("}") and words[i + 1][0] != "else":
        return True
    if words[i][1] is not NoteType.NORMAL: # 注释前换行
        return True
    if words[i + 1][1] is NoteType.NOTE: # 下一行是注释
        return True
    if words[i - 1][0] == "case" and words[i][0].endswith(":") and words[i+1][0] != "{": # case 语句
        return True
    if words[i][2] and words[i][0].endswith(","):
        return True
    return False

def combine_to_line(words: list[tuple[str, NoteType, bool]]) -> list[tuple[str, int]]:
    """
    合成一行
    """
    i: int = 0
    tab_level: int = 0
    last_tab_level: int = 0
    output_words: list[tuple[str, int]] = []
    line: str = ""
    while True:
        line += words[i][0]
        if i >= len(words) - 1:
            output_words.append((line, tab_level))
            break
        if words[i][0].endswith("{"):
            last_tab_level += 1
        if words[i+1][0].startswith("}") or words[i+1][0].endswith("}"):
            last_tab_level -= 1
        if is_end_of_line(words, i):
            output_words.append((line, tab_level))
            tab_level = last_tab_level
            line = ""
            i += 1
            continue
        line += " "
        i += 1
    return output_words

def combine_for_loop(words: list[tuple[str, int]]) -> None:
    """
    合并for循环
    """
    i: int = 0
    while i < len(words):
        if words[i][0].startswith("for ("):
            words[i] = (words[i][0] + " " + words[i+1][0] + " " + words[i+2][0], words[i][1])
            words.pop(i+1)
            words.pop(i+1)
        i += 1

def output(path: str, words: list[tuple[str, NoteType]]):
    """
    输出
    """
    combine_generic_angle_brackets(words)
    find_and_combine_ternary_operator(words)
    combine_colon(words)
    combine_parentheses(words)
    combine_square_brackets(words)
    combine_semicolon(words)
    combine_comma(words)
    combine_operator(words)
    combine_operator(words)
    words: list[tuple[str, NoteType, bool]] = find_array_comma(words)
    words: list[tuple[str, int]] = combine_to_line(words)
    combine_for_loop(words)
    with open(path, "w", encoding="utf-8") as f:
        for i in range(len(words)):
            f.write(words[i][1] * 4 * " " + words[i][0] + "\n")

def format_file(path: str):
    """
    整理代码
    """
    # 跳过部分已处理的文件
    # 留一个副本
    copy_path: str = os.path.abspath("copy\\" + os.path.relpath(path, os.getcwd()))
    copy_file: str = copy_path + ".copy"
    if os.path.exists(os.path.dirname(copy_file)) and os.path.exists(copy_file):
        if check_time(path, copy_file):
            return
    else:
        os.makedirs(os.path.dirname(copy_path), exist_ok=True)
    print("整理代码: ", path)
    shutil.copy(path, copy_file)
    with open(path, "r", encoding="utf-8") as f:
        data: str = f.read()
        # 分词
        words: list[tuple[str, NoteType]] = split_word(data)
        # 输出
        if path.endswith(".cs") or path.endswith(".gdshader"):  # {}类编程语言，如C#、gdshader
            output(path, words)
    # 使副本时间晚于原件
    time.sleep(0.01)
    os.utime(copy_file, (time.time(), time.time()))

def format_code(base_dir: str, current_dir: str, dir_list: list[str], ignore_list: list[str]):
    """
    整理代码
    """
    # 文件后缀白名单
    allow_suffix_list: list[str] = ["md", "cs", "gdshader"]
    for dir_name in dir_list:
        real_dir: str = os.path.join(current_dir, dir_name)
        relpath: str = os.path.relpath(real_dir, base_dir) + "\\"
        if any([i in relpath for i in ignore_list]):  # 忽略文件夹
            continue
        if os.path.isdir(real_dir):  # 如果是文件夹
            format_code(base_dir, real_dir, os.listdir(real_dir), ignore_list)
            continue
        if real_dir.split(".")[-1] not in allow_suffix_list:  # 如果不是白名单文件
            continue
        format_file(real_dir)

def format_whole_project() -> None:
    """
    整理项目代码
    - 其实就是格式化
    """
    current_directory: str = main.check_current_directory()
    ignore_list, _ = read_ignore(current_directory + "\\.gitignore")
    dir_list: list[str] = os.listdir(current_directory)
    # 将斜杠替换为反斜杠
    for i in range(len(ignore_list)):
        ignore_list[i] = ignore_list[i].replace("/", "\\")
    format_code(current_directory, current_directory, dir_list, ignore_list + [".git\\", "export\\"])
