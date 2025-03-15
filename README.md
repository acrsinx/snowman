# 雪人游戏
这是一个基于Godot引擎制作的游戏项目，游戏的核心玩法是玩家控制雪人完成剧情。游戏主要使用C#制作。  
## 报告错误
可以在Gitee的[这一页](https://gitee.com/acrsinx/snowman/issues)上报告。  
可以直接Gitee上使用Web IDE直接修改源文件并生成提交PR。  
## Godot版本
Godot版本参见[project.godot](project.godot)文件，目前是v4.2，未来可能会使用更高版本。  
## 剧情文件
剧情写在markdown文件中，位于plot/文件夹下，并需要运行pyscript/main.py以生成可用于游戏内的json文件。  
## 贡献
欢迎提交PR。
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
