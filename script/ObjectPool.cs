using Godot;
public partial class ObjectPool: object {
    public MultiMeshInstance3D instances;
    public bool[] haveUsed;
    public Vector3[] Velocities;
    public int Count {
        get => haveUsed.Length;
    }
    public ObjectPool(int length, Mesh mesh) {
        instances = new MultiMeshInstance3D() {
            Multimesh = new MultiMesh() {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = length,
                Mesh = mesh
            },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };
        Velocities = new Vector3[length];
        haveUsed = new bool[length];
    }
    public int HaveEmpty() {
        for (int i = 0; i < Count; i++) {
            if (!haveUsed[i]) {
                return i;
            }
        }
        return -1;
    }
    public int Add() {
        int empty = HaveEmpty();
        if (empty == -1) {
            return -1;
        }
        haveUsed[empty] = true;
        return empty;
    }
    public void Remove(int index) {
        haveUsed[index] = false;
    }
}
