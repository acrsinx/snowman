using Godot;
using System;
public partial class SnowCover: MeshInstance3D {
    public Player player;
    public SubViewport snowCoverTexture;
    public MultiMeshInstance2D snowmanStamp;
    private PlaneMesh mesh = null;
    public void Init(Player player) {
        this.player = player;
        player.ui.settingPanel.gameInformation.snowCover = this;
        mesh = (PlaneMesh) Mesh;
        UpdateMesh();
        snowCoverTexture = player.root.GetNode<SubViewport>("snowCover");
        snowmanStamp = snowCoverTexture.GetChild<MultiMeshInstance2D>(1);
        snowmanStamp.Multimesh.InstanceCount = 4;
    }
    public void Stamp(GameCharacter character, int id) {
        float x = character.Velocity.Z;
        float y = -character.Velocity.X;
        float size = Map.mapSizes[player.ui.currentScene];
        snowmanStamp.Multimesh.SetInstanceTransform2D(id, new Transform2D(MathF.Atan2(y, x), player.ui.settingPanel.gameInformation.SnowCoverSize / size * new Vector2(character.GlobalPosition.X, character.GlobalPosition.Z)).ScaledLocal(Tool.Vector2(0.06f * player.ui.settingPanel.gameInformation.SnowCoverSize / size)));
    }
    public void RefreshSnowCover() {
        UpdateMesh();
        snowCoverTexture.RenderTargetClearMode = SubViewport.ClearMode.Once;
    }
    public void SetSubDivide(int subdivide) {
        mesh.SubdivideWidth = subdivide;
        mesh.SubdivideDepth = subdivide;
    }
    public void SetSnowCoverSize(int size) {
        snowCoverTexture.Size = Tool.Vector2I(size);
        snowCoverTexture.RenderTargetClearMode = SubViewport.ClearMode.Once;
    }
    private void UpdateMesh() {
        mesh.Size = new Vector2(Map.mapSizes[player.ui.currentScene], Map.mapSizes[player.ui.currentScene]);
    }
    public static bool IsOnSnowCover(Vector3 globalPosition) {
        return MathF.Abs(globalPosition.Y) < 0.05f;
    }
}
