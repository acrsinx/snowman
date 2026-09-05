"""
统计部分项目数据
"""
import os

from pyscript.formatCode import is_ignore_dir, get_ignore_list
from pyscript.main import check_current_directory

code_type: dict = {}

not_text_suffix_list: tuple = (".png", ".mp3")

def count(path: str, ignore_list) -> None:
    """
    统计部分项目数据
    """
    global code_type
    # 迭代文件夹
    for file in os.listdir(path):
        if is_ignore_dir(file + "\\", ignore_list):
            continue
        # 判断是否为文件夹
        sub_file_name = os.path.join(path, file)
        if os.path.isdir(sub_file_name):
            count(sub_file_name, ignore_list)
            continue
        file_type: str = file.split('.')[-1]
        line: int = 0
        if not sub_file_name.endswith(not_text_suffix_list):
            with open(sub_file_name, 'r', encoding="utf-8") as f:
                line = len(f.readlines())
        if file_type in code_type:
            code_type[file_type] = (code_type[file_type][0] + 1, code_type[file_type][1] + line)
        else:
            code_type[file_type] = (1, line)

if __name__ == '__main__':
    count(check_current_directory(), get_ignore_list())
    sorted_data = dict(sorted(code_type.items(), key=lambda item: item[1][1], reverse=True))
    print(sorted_data)
