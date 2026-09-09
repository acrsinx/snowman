using Godot;
using Godot.Collections;
public partial class MultiMeshMaker: Node3D, Initable {
    [Export] public bool addProgrammaticStuffs = false;
    [Export] public int seed = 42;
    [Export] public int count = 10;
    public Array<MeshInstance3D> nodes = new();
    private Array<Array<MeshInstance3D>> meshes = new();
    public Array<StaticBody3D> staticBodies = new();
    public void Init() {
        // 找到所有的子节点
        FindMeshes(this);
        if (nodes.Count <= 0) {
            Ui.Log("没有找到任何可以合并的 MeshInstance3D 节点", Name);
            return;
        }
        meshes.Add(new Array<MeshInstance3D>() {
            nodes[0]
        });
        bool added;
        for (int i = 1; i < nodes.Count; i++) {
            added = false;
            for (int j = 0; j < meshes.Count; j++) {
                if (meshes[j][0].Mesh == nodes[i].Mesh) {
                    meshes[j].Add(nodes[i]);
                    added = true;
                    break;
                }
            }
            if (!added) {
                meshes.Add(new Array<MeshInstance3D>() {
                    nodes[i]
                });
            }
        }
        int totalCount = nodes.Count + (addProgrammaticStuffs?count:0);
        MultiMesh multiMesh = new() {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            InstanceCount = totalCount,
            Mesh = meshes[0][0].Mesh
        };
        for (int i = 0; i < meshes[0].Count; i++) {
            multiMesh.SetInstanceTransform(i, meshes[0][i].GlobalTransform);
        }
        if (addProgrammaticStuffs) {
            System.Random random = new(seed);
            Basis basis = Basis.FromScale(Vector3.One * 0.3f);
            for (int i = nodes.Count; i < totalCount; i++) {
                Vector3 position = new(random.NextSingle() * 15 - 7, random.NextSingle() * 0.1f - 0.16f, random.NextSingle() * 15 - 7);
                multiMesh.SetInstanceTransform(i, new Transform3D(basis, position));
            }
        }
        MultiMeshInstance3D firstMultiMeshInstance = new() {
            Multimesh = multiMesh
        };
        GetParent().AddChild(firstMultiMeshInstance);
        for (int i = 0; i < staticBodies.Count; i++) {
            staticBodies[i].Reparent(firstMultiMeshInstance);
        }
        for (int i = 1; i < meshes.Count; i++) {
            multiMesh = new() {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
                InstanceCount = meshes[i].Count,
                Mesh = meshes[i][0].Mesh
            };
            for (int j = 0; j < meshes[i].Count; j++) {
                multiMesh.SetInstanceTransform(j, meshes[i][j].GlobalTransform);
            }
            MultiMeshInstance3D multiMeshInstance = new() {
                Multimesh = multiMesh
            };
            firstMultiMeshInstance.AddChild(multiMeshInstance);
        }
        for (int i = 0; i < staticBodies.Count; i++) {
            staticBodies[i].Reparent(firstMultiMeshInstance);
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
