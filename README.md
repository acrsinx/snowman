# 化雪（Thawing）
化雪（Thawing）是一个基于Godot引擎制作的游戏项目，游戏的核心玩法是玩家控制雪人完成剧情。游戏主要使用C#制作。  
## 报告错误
可以在Gitee的[这一页](https://gitee.com/acrsinx/snowman/issues)上报告。  
## Godot版本
Godot版本参见[project.godot](project.godot)文件，目前是v4.2，未来可能会使用更高版本。  
## 剧情文件
剧情写在markdown文件中，位于plot/文件夹下，并需要运行pyscript/main.py以生成可用于游戏内的json文件。  
## 贡献
欢迎贡献。  
- 可以直接Gitee上使用Web IDE直接修改源文件并生成提交PR。  
- 注意，在条件允许的情况下，克隆项目后及提交PR前请先运行pyscript/main.py以自动整理并生成剧情文件，确保文件应有尽有，风格一致。  
- 当然，该文件并不能正确处理所有情况，请自行为switch-case语句增加完整的大括号。如下：  
```csharp
switch (a) {
    case 1: {
        break;
    }
    default: {
        break;
    }
}
```
### 修复错别字
1. 定位错别字。  
    在游戏剧情中遇到错别字时，使用快捷键`F3`打开调试信息（手机点击右下角的小按钮），在调试信息中找到错别字的文件名。  
    注意，调试信息中显示的文件名编译后的文件名，需要找到对应的源文件。如：`plotJson\plot0\plot0_2.json`对应`plot\plot0.md`。  
1. 打开对应的源文件，找到错别字，大部分软件都可以使用快捷键`Ctrl+F`搜索。  
1. 修改错别字，并保存文件。  
1. 在[这里](https://gitee.com/acrsinx/snowman/pulls)提交PR。  
### 参与开发
1. 配置环境。  
    [在这里](https://godotengine.org/download/archive/)下载安装对应版本的Godot。  
    配置Python环境，建议是`Python 3.9`。  
    下载安装`git`。  
1. 克隆仓库。  
    ```bash
    git clone https://gitee.com/acrsinx/snowman.git
    ```
1. 运行[pyscript/main.py](pyscript/main.py)以生成剧情文件。  
1. 在`Godot`或其它代码编辑器中修改源文件。  
1. 在[这里](https://gitee.com/acrsinx/snowman/pulls)提交PR。  
