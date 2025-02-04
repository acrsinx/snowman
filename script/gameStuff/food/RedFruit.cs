using Godot;
public partial class RedFruit: Food {
    public RedFruit(Ui ui): base(ui) {
    }
    public override bool CanUse() {
        return !ui.playerCamera.playerCharacter.health.IsFullHealth();
    }
    public override void Use() {
        ui.playerCamera.playerCharacter.health.CurrentHealth += 10;
    }
    public override string GetName() {
        return "红色的水果";
    }
    public override Texture2D GetTexture() {
        return Images.RedFruit;
    }
}
