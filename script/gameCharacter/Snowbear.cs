using Godot;
public partial class Snowbear: GameCharacter {
    public static PackedScene SnowbearScene = GD.Load<PackedScene>("res://model/snowbear.gltf");
    public Snowbear(Player player): base(SnowbearScene, player, new SphereShape3D() {
        Radius = 0.5f
    }, new Vector3(0, 0.5f, 0), true) {
        auto = new AutoCharacterManager(this, player);
        auto.afterAttack += () => {
            player.character.BeAttack((int)(20.0f / character.GlobalPosition.DistanceTo(player.character.GlobalPosition)), DamageType.sound, isEnemy);
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
        GetPlotCharacter().PlayAnimation("attack");
    }
    public override void PlayWalkAnimation() {
        if (GetPlotCharacter().GetAnimationName() == "walk") {
            return;
        }
        GetPlotCharacter().PauseAnimation();
        GetPlotCharacter().PlayAnimation("walk");
    }
}
