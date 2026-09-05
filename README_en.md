
[简体中文](README_zh_CN.md)  

# Thawing
Thawing is a game project made with the Godot engine, in which player can control a snowman to complete the plot. The game is mainly made with C#.  
## Report Bugs
You can set up an issue on [this page](https://github.com/acrsinx/snowman/issues) or [this page](https://gitee.com/acrsinx/snowman/issues) via Gitee via GitHub.  
## Godot Version
It's v4.2 now, and it may be updated to a higher version in the future. See also [project.godot](project.godot).  
## Where's the Plot?
The plot is written in markdown files located in the `plot/` folder. It's necessary to run `pyscript/main.py` to generate json files that can be read in the game.   
## Make Contributions
Welcome to contribute.  
- Use the Web editor on GitHub or Web IDE on Gitee to edit and submit PR.  
- If possible, after cloning or before submitting a PR, run `pyscript/main.py` to format, generate and check the files.  
- Indeed, that program can't handle all the suituations well. Please add complete braces for switch-case statements manually. For example:  
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
### Fix Misspelling Errors
1. Find its source file.  
    When encountering a misspelling error in the game plot, use the shortcut key `F3` to open the debug information (click the small button in the lower right corner on the phone). The source file name is displayed in the debug information.  
	However, the file name there is the "compiled" file name, so you need to find the corresponding source file. For example: `plotJson\plot0\plot0_2.json` corresponds to `plot\plot0.md`.  
1. Open the source file, and find the misspelling. Most software can use the shortcut key `Ctrl+F` to search.  
1. Fix it and save the file.  
1. Submit a PR via [GitHub](https://github.com/acrsinx/snowman/pulls) or [Gitee](https://gitee.com/acrsinx/snowman/pulls).  
### Take Part in Development
1. Set up the environment.  
    Download and install the correct version of Godot from [here](https://godotengine.org/download/archive/). Note: It's the version that supports `.NET`.  
	Set up the environment for Python. `Python 3.9` preferred.  
    Download and install `.NET` from [here](https://dotnet.microsoft.com/download).  
    Download and install `Git`.  
1. Fork the repository.  
1. Clone it.  
1. Run `pyscript/main.py` to generate plot files.  
1. Edit code or other files in whatever way you like.  
1. Submit a PR via [GitHub](https://github.com/acrsinx/snowman/pulls) or [Gitee](https://gitee.com/acrsinx/snowman/pulls).  
