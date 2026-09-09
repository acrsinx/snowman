using Godot;
using System;
public partial class FenceMaker: Node3D {
    private static Random random;
    private static readonly Vector3 range = Tool.Vector3(0.1f);
    private static readonly Vector3 rangeRotation = Tool.Vector3(0.1f);
    private static readonly Vector3[] yShift = {
        new(0, 0.2f, 0),
        new(0, 0.4f, 0)
    };
    private static readonly Vector3[] nailShift = {
        new(-0.015f, 0.15f, 0),
        new(-0.015f, 0.35f, 0)
    };
    private static readonly PackedScene fence1 = ResourceLoader.Load<PackedScene>("res://model/fence1.glb");
    private static readonly PackedScene fence2 = ResourceLoader.Load<PackedScene>("res://model/fence2.glb");
    private static readonly PackedScene nail = ResourceLoader.Load<PackedScene>("res://model/nail.glb");
    [Export] public Vector3 start;
    [Export] public Vector3 end;
    [Export] public int seed = 42;
    public override void _Ready() {
        random = new(seed);
        Vector3 direction = end - start;
        float distance = direction.Length();
        int total = Mathf.CeilToInt(distance / 0.4f) + 1;
        Vector3[,] nailPosition = new Vector3[2, total];
        Vector3[,] nailTipPosition = new Vector3[2, total];
        for (int i = 0; i < total; i++) {
            Vector3 position = start + direction * ((float) i / total);
            if (random.Next(0, 1) == 0) {
                position += Tool.RandomVector3(range, random);
            }
            Vector3 rotation = new(0, Tool.AngleXZ(direction), 0);
            if (random.Next(0, 1) == 0) {
                rotation += Tool.RandomVector3(rangeRotation, random);
            }
            nailPosition[0, i] = position + Tool.Rotate(yShift[0], rotation);
            nailPosition[1, i] = position + Tool.Rotate(yShift[1], rotation);
            nailTipPosition[0, i] = position + Tool.Rotate(nailShift[0], rotation);
            nailTipPosition[1, i] = position + Tool.Rotate(nailShift[1], rotation);
            if (random.Next(0, 100) == 37) {
                rotation += new Vector3(0, 0, MathF.PI);
                position += new Vector3(0, 0.5f, 0);
            }
            Node3D fence = (Node3D) fence1.Instantiate();
            AddChild(fence);
            fence.GlobalPosition = position;
            Tool.Rotate(fence, rotation);
        }
        for (int i = 0; i < total - 1; i++) {
            if (random.Next(0, 10) == 3) {
                continue;
            }
            Node3D fence = (Node3D) fence2.Instantiate();
            AddChild(fence);
            Vector3 position = (nailPosition[i % 2, i + 1] + nailPosition[i % 2, i]) * 0.5f;
            Vector3 rotation = new(0, Tool.AngleXZ(nailPosition[i % 2, i + 1] - nailPosition[i % 2, i]), 0);
            if (random.Next(0, 5) == 3) {
                rotation += new Vector3(0, 0, MathF.PI);
            }
            if (random.Next(0, 5) == 3) {
                rotation += new Vector3(0, MathF.PI, 0);
            }
            fence.GlobalPosition = position;
            fence.Rotation = rotation;
        }
        for (int i = 0; i < total * 2; i++) {
            if (random.Next(0, 100) == 37) {
                continue;
            }
            Node3D nailNode = (Node3D) nail.Instantiate();
            AddChild(nailNode);
            nailNode.GlobalPosition = nailTipPosition[i % 2, i / 2];
            Vector3 nailRotation = Tool.RandomVector3(Tool.Vector3(MathF.PI), random);
            Tool.Rotate(nailNode, nailRotation);
        }
    }
}
