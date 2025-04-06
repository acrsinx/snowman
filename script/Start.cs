using Godot;
public partial class Start : Node3D {
    private const float MinX = -1.3f;
    private const float MaxX = -0.3f;
    private const float MidX = (MinX + MaxX) / 2.0f;
    private const float Rx = (MaxX - MinX) / 2.0f;
    public override void _Process(double delta) {
        float t = Time.GetTicksMsec() / 5000.0f;
        Position = new Vector3(
            Mathf.Sin(t) * Rx + MidX,
            0.0f,
            0.0f
        );
    }
}
