using Godot.Collections;

public class CaptionResource : object {
    public int id;
    public string actorName;
    public string caption;
    public int time;
    public bool canChoose;
    public string[] choose;
    public DoSomething next;
    public DoSomething[] todoAfterChoose;
    public Ui ui;
    public CaptionResource(Ui ui, Dictionary dict, int id) {
        this.id = id;
        this.ui = ui;
        actorName = (string) dict["actorName"];
        caption = (string) dict["caption"];
        time = caption.Length * 200;
        if (dict.ContainsKey("next")) {
            Dictionary nextToDo = (Dictionary) dict["next"];
            next = DoSomething.ToDoSomething(nextToDo, ui);
        }
        canChoose = dict.ContainsKey("0");
        if (canChoose) {
            int chooseLength = 1;
            if (dict.ContainsKey("1")) {
                chooseLength = 2;
            }
            if (dict.ContainsKey("2")) {
                chooseLength = 3;
            }
            choose = new string[chooseLength];
            todoAfterChoose = new DoSomething[chooseLength];
            for (int i = 0; i < chooseLength; i++) {
                choose[i] = (string) ((Dictionary)dict[i.ToString()])["text"];
                todoAfterChoose[i] = DoSomething.ToDoSomething((Dictionary) ((Dictionary)dict[i.ToString()])["next"], ui);
            }
        }
    }
    public Dictionary Save() {
        Dictionary dict = new() {
            ["actorName"] = actorName,
            ["caption"] = caption,
            ["time"] = time,
            ["next"] = next.ToDictionary()
        };
        if (canChoose) {
            for (int i = 0; i < choose.Length; i++) {
                dict[i] = choose[i];
            }
        }
        return dict;
    }
}
