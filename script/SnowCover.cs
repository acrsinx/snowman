using Godot;
using System;
public partial class SnowCover: MeshInstance3D {
    public Player player;
    public SubViewport snowCoverTexture;
    public MultiMeshInstance2D snowmanStamp;
    public MultiMeshInstance2D snowbearStamp;
    private PlaneMesh mesh = null;
    public void Init(Player player) {
        this.player = player;
        player.ui.settingPanel.gameInformation.snowCover = this;
        mesh = (PlaneMesh) Mesh;
        UpdateMesh();
        snowCoverTexture = player.root.GetNode<SubViewport>("snowCover");
        snowmanStamp = snowCoverTexture.GetChild<MultiMeshInstance2D>(1);
        snowmanStamp.Multimesh = new() {
            InstanceCount = 4,
            Mesh = new QuadMesh() {
                Size = new Vector2(16, 16)
            }
        };
        snowbearStamp = snowCoverTexture.GetChild<MultiMeshInstance2D>(2);
        snowbearStamp.Multimesh = new() {
            InstanceCount = 2,
            Mesh = new QuadMesh() {
                Size = new Vector2(16, 16)
            }
        };
    }
    /// <summary>
    /// 雪地印
    /// </summary>
    public void Stamp(GameCharacter character, Transform2D transform2D) {
        switch (character.type) {
            case GameCharacter.CharacterType.Snowman: {
                snowmanStamp.Multimesh.SetInstanceTransform2D(character.GetID(), transform2D);
                break;
            }
            case GameCharacter.CharacterType.Snowbear: {
                snowbearStamp.Multimesh.SetInstanceTransform2D(character.GetID(), transform2D);
                break;
            }
        }
    }
    public static Transform2D GetStampTransform2D(Player player, Vector3 position, float scale, Vector3 velocity = default) {
        float size = Map.mapSizes[player.ui.currentScene];
        float direction = 0;
        if (velocity != default) {
            float x = velocity.Z;
            float y = -velocity.X;
            direction = MathF.Atan2(y, x);
        }
        return new Transform2D(direction, player.ui.settingPanel.gameInformation.SnowCoverSize / size * new Vector2(position.X, position.Z))
        .ScaledLocal(Tool.Vector2(scale * player.ui.settingPanel.gameInformation.SnowCoverSize / size));
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
        ClearStamps();
    }
    private void UpdateMesh() {
        mesh.Size = new Vector2(Map.mapSizes[player.ui.currentScene], Map.mapSizes[player.ui.currentScene]);
        ClearStamps();
    }
    private void ClearStamps() {
        if (snowmanStamp == null) {
            return;
        }
        for (int i = 0; i < snowmanStamp.Multimesh.InstanceCount; i++) {
            snowmanStamp.Multimesh.SetInstanceTransform2D(i, Tool.ZeroTransform2D);
        }
    }
    public static bool IsOnSnowCover(Vector3 globalPosition) {
        return MathF.Abs(globalPosition.Y) < 0.05f;
    }
}
