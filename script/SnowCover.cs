using Godot;
using System;
public partial class SnowCover: MeshInstance3D {
    public Player player;
    public SubViewport snowCoverTexture;
    public MultiMeshInstance2D snowmanStamp;
    private PlaneMesh mesh = null;
    public void Init(Player player) {
        this.player = player;
        mesh = (PlaneMesh)Mesh;
        UpdateMesh();
        mesh.SubdivideWidth = 49;
        mesh.SubdivideDepth = 49;
        snowCoverTexture = player.root.GetNode<SubViewport>("snowCover");
        snowmanStamp = snowCoverTexture.GetChild<MultiMeshInstance2D>(1);
        snowmanStamp.Multimesh.InstanceCount = 4;
    }
    public void Stamp(Node3D character, float x, float y) {
        float size = Map.mapSizes[player.ui.currentScene];
        snowmanStamp.Multimesh.SetInstanceTransform2D(0, new Transform2D(MathF.Atan2(y, x), 64 / size * new Vector2(character.GlobalPosition.X, character.GlobalPosition.Z))
        .ScaledLocal(Tool.Vector2(6/size)));
    }
    public void RefreshSnowCover()
    {
        UpdateMesh();
        snowCoverTexture.RenderTargetClearMode = SubViewport.ClearMode.Once;
    }
    private void UpdateMesh()
    {
        mesh.Size = new Vector2(Map.mapSizes[player.ui.currentScene], Map.mapSizes[player.ui.currentScene]);
    }
}
