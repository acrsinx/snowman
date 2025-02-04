using Godot;
public partial interface HaveCharacter {
    public GameCharacter GetCharacter();
    public static HaveCharacter GetHaveCharacter(Node node) {
        Node n = node;
        for (int i = 0; i < 10; i++) {
            if (n is null) {
                return null;
            }
            if (n is HaveCharacter h) {
                return h;
            }
            n = n.GetParent();
        }
        return null;
    }
    public static PhysicsBody3D GetPhysicsBody3D(Node node) {
        if (node is null) {
            return null;
        }
        if (node is PhysicsBody3D physicsBody3D) {
            return physicsBody3D;
        }
        foreach (Node n in node.GetChildren()) {
            PhysicsBody3D p = GetPhysicsBody3D(n);
            if (p != null) {
                return p;
            }
        }
        Node n1 = node.GetParent();
        for (int i = 0; i < 10; i++) {
            if (n1 is null) {
                return null;
            }
            if (n1 is PhysicsBody3D p2) {
                return p2;
            }
            n1 = n1.GetParent();
        }
        return null;
    }
}
