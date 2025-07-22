"""
运行代码格式化，生成翻译文件，生成剧情文件
"""
import os

import formatCode
import makeTranslate
import makePlotJson

need_dirs: list[str] = ["export\\export\\", "localization\\template\\plot\\"]

def check_current_directory() -> str:
    """
    设置正确的工作目录
    """
    os.chdir(os.path.dirname(os.path.dirname(__file__)))
    return os.getcwd()

def make_dir() -> None:
    """
    生成必要的目录
    """
    for dir_name in need_dirs:
        if not os.path.exists(dir_name):
            os.makedirs(dir_name)
            print("创建目录: ", dir_name)

if __name__ == '__main__':
    # 检查目录是否正确
    check_current_directory()
    # 生成必要的目录
    make_dir()
    # 整理代码
    formatCode.format_whole_project()
    # 生成剧本json文件
    makePlotJson.make_json()
    # 创建翻译模板
    makeTranslate.create_localization_template_all()
    # 生成翻译json文件
    makeTranslate.make_translate_all()
    # 将翻译文件复制到用户数据目录
    makeTranslate.copy_translate_to_user_data()
