using Godot;
public partial class Snowbear: GameCharacter {
    public static readonly PackedScene SnowbearScene = GD.Load<PackedScene>("res://model/snowbear.glb");
    public Snowbear(Player player): base(SnowbearScene, player, new SphereShape3D() {
        Radius = 0.5f
    }, new Vector3(0, 0.5f, 0), true) {
        PlotCharacter.AddAnimationPlayer(this, "fourFeet");
        auto = new AutoCharacterManager(this, player);
        auto.afterAttack += () => {
            // 产生声波
            SoundWave soundWave = new(player, GlobalPosition) {
                GlobalPosition = character.GlobalPosition
            };
            GetPlotCharacter().PauseAnimation();
        };
        health.MaxHealth = 100;
        health.SetFullHealth();
        health.die += () => {
            QueueFree();
        };
    }
    public override int GetAttackWaitTime() {
        return 500;
    }
    public override void CharacterAttack() {
        base.CharacterAttack();
        auto.Attack();
        GetPlotCharacter().PauseAnimation();
        GetPlotCharacter().PlayAnimation("fourFeet/attack");
    }
    public override void PlayWalkAnimation() {
        if (GetPlotCharacter().GetAnimationName() == "fourFeet/walk") {
            return;
        }
        GetPlotCharacter().PauseAnimation();
        GetPlotCharacter().PlayAnimation("fourFeet/walk");
    }
}
