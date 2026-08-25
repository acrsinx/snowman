using Godot;
using System;
public partial class SnowCover: MeshInstance3D {
    public Player player;
    public SubViewport snowCoverTexture;
    public MultiMeshInstance2D snowmanStamp;
    public override void _Ready() {
        Node r = GetTree().Root.GetChild(0);
        if (r.FindChild("ui") == null) {
            player = null;
            return;
        }
        player = r.GetNode<Ui>("ui").player;
        Mesh = new PlaneMesh() {
            Size = new Vector2(Map.mapSizes[0], Map.mapSizes[0]),
            SubdivideWidth = 49,
            SubdivideDepth = 49
        };
        snowCoverTexture = player.root.GetNode<SubViewport>("snowCover");
        snowmanStamp = snowCoverTexture.GetChild<MultiMeshInstance2D>(1);
        snowmanStamp.Multimesh.InstanceCount = 1;
        ((ShaderMaterial) MaterialOverride).SetShaderParameter("height", snowCoverTexture.GetTexture());
        player.snowCover = this;
    }
    public void Stamp(Node3D character, float x, float y) {
        snowmanStamp.Multimesh.SetInstanceTransform2D(0, new Transform2D(MathF.Atan2(y, x), 64 / Map.mapSizes[0] * new Vector2(character.GlobalPosition.X, character.GlobalPosition.Z)).ScaledLocal(new Vector2(0.3f, 0.3f)));
    }
}
