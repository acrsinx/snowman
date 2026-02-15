using Godot;
using Godot.Collections;
public partial class MultiMeshMaker: Node3D, Initable {
    public Array<MeshInstance3D> nodes = new();
    public Array<StaticBody3D> staticBodies = new();
    public void Init() {
        // 找到所有的子节点
        FindMeshes(this);
        if (nodes.Count <= 0) {
            Ui.Log("没有找到任何可以合并的 MeshInstance3D 节点", Name);
            return;
        }
        // 检查是否都是同样的 Mesh
        Mesh mesh = nodes[0].Mesh;
        for (int i = 1; i < nodes.Count; i++) {
            if (nodes[i].Mesh != mesh) {
                Ui.Log("所有 MeshInstance3D 节点必须使用相同的 Mesh", Name, i);
                return;
            }
        }
        // 生成 MultiMeshInstance3D
        MultiMesh multiMesh = new() {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            InstanceCount = nodes.Count,
            Mesh = mesh
        };
        for (int i = 0; i < nodes.Count; i++) {
            multiMesh.SetInstanceTransform(i, nodes[i].GlobalTransform);
        }
        MultiMeshInstance3D multiMeshInstance = new() {
            Multimesh = multiMesh
        };
        GetParent().AddChild(multiMeshInstance);
        for (int i = 0; i < staticBodies.Count; i++) {
            staticBodies[i].Reparent(multiMeshInstance);
        }
        // 删除原来的节点
        QueueFree();
    }
    public void FindMeshes(Node node) {
        for (int i = 0; i < node.GetChildCount(); i++) {
            Node child = node.GetChild(i);
            FindMeshes(child);
            if (child is MeshInstance3D meshInstance) {
                nodes.Add(meshInstance);
            }
            if (child is StaticBody3D staticBody) {
                staticBodies.Add(staticBody);
            }
        }
    }
}
