import os

import arrangeCode
import makeTranslate
import makePlotJson

def check_current_directory() -> str:
    """
    设置正确的工作目录
    """
    os.chdir(os.path.dirname(os.path.dirname(__file__)))
    return os.getcwd()

need_dirs: list[str] = ["export\\export\\", "localization\\template\\plot\\"]

def make_dir() -> None:
    for dir in need_dirs:
        if not os.path.exists(dir):
            os.makedirs(dir)
            print("创建目录: ", dir)

if __name__ == '__main__':
    # 检查目录是否正确
    check_current_directory()
    # 生成必要的目录
    make_dir()
    # 整理代码
    arrangeCode.arrange_whole_project()
    # 生成剧本json文件
    makePlotJson.make_json()
    # 创建翻译模板
    makeTranslate.create_localization_template_all()
    # 生成翻译json文件
    makeTranslate.make_translate_all()
