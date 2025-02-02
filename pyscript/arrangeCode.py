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

def arrange(path: str):
    """
    整理代码
    """
    print(path)
    with open(path, "r", encoding="utf-8") as f:
        pass

def arrange_code(base_dir: str, current_dir: str, dir_list: list[str], ignore_list: list[str]):
    """
    整理代码
    """
    # 文件后缀白名单
    allow_suffix_list: list[str] = ["md", "cs", "gdshader"]
    for dir in dir_list:
        real_dir: str = current_dir + "\\" + dir
        if os.path.relpath(real_dir, base_dir)+"\\" in ignore_list: # 忽略文件夹
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
