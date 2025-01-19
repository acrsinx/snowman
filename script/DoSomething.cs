using System;
using Godot;
using Godot.Collections;

public abstract class DoSomething {
    public Ui ui;
    public virtual void Do(Plot plot, int captionIndex) {}
    public abstract Dictionary ToDictionary();
    public static DoSomething ToDoSomething(Dictionary dict, Ui ui) {
        if (dict.ContainsKey("caption")) {
            return new DoCaption(ui, dict);
        } else if (dict.ContainsKey("exit")) {
            return new DoExit(ui, dict);
        } else {
            throw new Exception("不知道这个剧情元结束后要做什么。");
        }
    }
}
