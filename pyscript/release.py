"""
发行版导出
"""
import os

import main

if __name__ == '__main__':
    # 检查目录是否正确
    dir: str = main.check_current_directory()
    # 生成必要的目录
    main.make_dir()
    # 整理代码
    main.formatCode.format_whole_project(True)
    # 生成剧本json文件
    main.makePlotJson.make_json(True)
    # 创建翻译模板
    main.makeTranslate.create_localization_template_all(True)
    # 生成翻译json文件
    main.makeTranslate.make_translate_all(True)
    # 将翻译文件复制到用户数据目录
    main.makeTranslate.copy_translate_to_user_data()
    # 使用命令行生成发行版
    try:
        godot_path: str = input("请输入 Godot 的路径：")
        code1: str = godot_path + " --headless --import \"" + dir + "\\project.godot\" --export-release \"Windows Desktop\" --quit"
        code2: str = godot_path + " --headless --import \"" + dir + "\\project.godot\" --export-release \"Android\" --quit"
        os.system(code1)
        os.system(code2)
    except Exception as e:
        print(e)
