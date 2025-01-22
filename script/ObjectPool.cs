using Godot;

public partial class ObjectPool : object{
    public bool[] haveUsed;
    public Node3D[] list;
    public RigidBody3D[] rList;
    public int Count{
        get => list.Length;
    }
    public ObjectPool(int length) {
        list = new Node3D[length];
        rList = new RigidBody3D[length];
        haveUsed = new bool[length];
    }
    public int HaveEmpty() {
        for (int i = 0; i < haveUsed.Length; i++) {
            if (!haveUsed[i]) {
                return i;
            }
        }
        return -1;
    }
    public Node3D Add(PackedScene item) {
        int empty = HaveEmpty();
        if (empty == -1) {
            return null;
        }
        list[empty] = item.Instantiate<Node3D>();
        rList[empty] = list[empty].GetChild<RigidBody3D>(0);
        haveUsed[empty] = true;
        return list[empty];
    }
    public Node3D Get(int index) {
        return list[index];
    }
    public void Remove(int index) {
        haveUsed[index] = false;
    }
}
