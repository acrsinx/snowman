using System;
using Godot;
public class CameraManager: object {
    private readonly Camera3D camera;
    public readonly Marker3D cameraMarker;
    public readonly RayCast3D cameraRay;
    public readonly Player player;
    /// <summary>
    /// 相机标志的原位置
    /// </summary>
    public static readonly Vector3 CameraMarkerOrigin = new(0, 1.3f, 0);
    /// <summary>
    /// 相机标志的最小合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationMinX = -0.5f;
    /// <summary>
    /// 相机标志的最大合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationMaxX = 0.2f;
    public Shake cameraShake = new();
    private static Vector3 cameraVector = new(0.31f, 0, 1);
    public static readonly Vector3[] checkList = {
        new(-0.15f, 0, 0),
        new(0.15f, 0, 0),
        new(0, -0.15f, 0),
        new(0, 0.15f, 0),
        new(0, 0, -0.15f),
        new(0, 0, 0.15f),
        new(-0.15f, -0.15f, 0),
        new(-0.15f, 0.15f, 0),
        new(0.15f, -0.15f, 0),
        new(0.15f, 0.15f, 0),
        new(-0.15f, 0, -0.15f),
        new(-0.15f, 0, 0.15f),
        new(0.15f, 0, -0.15f),
        new(0.15f, 0, 0.15f),
        new(0, -0.15f, -0.15f),
        new(0, -0.15f, 0.15f),
        new(0, 0.15f, -0.15f),
        new(0, 0.15f, 0.15f)
    };
    private const float maxDistance = 3.0f;
    private float distance = 3.0f;
    public CameraManager(Camera3D camera, RayCast3D cameraRay, Player player, Marker3D cameraMarker) {
        this.camera = camera;
        this.cameraRay = cameraRay;
        this.player = player;
        this.cameraMarker = cameraMarker;
        SetCameraPosition();
        SetFov();
    }
    private void SetFov() {
        float fov = 75 + (distance - 5) * 5;
        if (fov < 1 || fov > 179) {
            fov = 75;
        }
        camera.Fov = fov;
    }
    // 获取相机全局位置
    public Vector3 GetCameraGlobalPosition() {
        return camera.GlobalPosition;
    }
    private bool IsCameraTouching() {
        for (int i = 0; i < checkList.Length; i++) {
            cameraRay.TargetPosition = checkList[i];
            cameraRay.ForceRaycastUpdate();
            GodotObject collider = cameraRay.GetCollider();
            if (collider != null) {
                if (collider.GetType().FullName != "Camera") {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 将相机位置与方向重置
    /// </summary>
    public void SetCameraPosition() {
        camera.Position = cameraVector * distance;
        camera.Rotation = Vector3.Zero;
    }
    /// <summary>
    /// 玩家移动时回正相机
    /// </summary>
    /// <param name="fDelta">时间增量</param>
    public void UpdateCameraWhenMoving(float fDelta) {
        // 回正视场角
        if (distance < maxDistance) {
            float record = distance;
            distance += fDelta * 3;
            distance = Math.Min(distance, maxDistance);
            SetCameraPosition();
            if (IsCameraTouching()) {
                distance = record;
                SetCameraPosition();
            }
        }
        // 回正相机角度
        if (cameraMarker.Rotation.X > CameraMarkerRotationMaxX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatTo(cameraMarker.Rotation.X, CameraMarkerRotationMaxX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (cameraMarker.Rotation.X < CameraMarkerRotationMinX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatTo(cameraMarker.Rotation.X, CameraMarkerRotationMinX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        SetFov();
    }
    /// <summary>
    /// 旋转视角
    /// </summary>
    /// <param name="mouseMove">鼠标位移</param>
    public void UpdateCameraWhenTurning(Vector2 mouseMove) {
        // 处理cameraMarker.Rotation
        cameraMarker.Rotation = new Vector3(cameraMarker.Rotation.X + mouseMove.Y, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        // 处理player.Rotation
        player.Rotation = new Vector3(player.Rotation.X, player.Rotation.Y + mouseMove.X, player.Rotation.Z);
        // 限制视角
        if (-1.2f > cameraMarker.Rotation.X) {
            cameraMarker.Rotation = new Vector3(-1.2f, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        } else if (0.5f < cameraMarker.Rotation.X) {
            cameraMarker.Rotation = new Vector3(0.5f, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (player.ui.uiType == UiType.computer) {
            // 鼠标归中
            Input.WarpMouse(0.5f * player.GetViewport().GetVisibleRect().Size);
        }
    }
    /// <summary>
    /// 刷新相机
    /// </summary>
    public void UpdateCamera() {
        if (distance > maxDistance) {
            distance = maxDistance;
            SetCameraPosition();
        } else if (distance < 0) {
            distance = 1;
            SetCameraPosition();
        }
        if (IsCameraTouching()) { // 相机穿模了
            camera.Position = Vector3.Zero;
            // 后移相机
            while (camera.Position.Z < maxDistance) {
                camera.Position += cameraVector * 0.2f;
                if (IsCameraTouching()) { // 如果碰到物体，则停止
                    camera.Position -= cameraVector * 0.2f;
                    distance = camera.Position.Z;
                    break;
                }
            }
        }
        // 相机震动
        cameraMarker.Position = cameraShake.GetShakeOffset(player.ui.totalGameTime) + CameraMarkerOrigin;
        SetFov();
        player.character.Visible = camera.Position.Z > 2;
    }
    public void WheelUp() {
        distance -= 0.2f;
        distance = MathF.Max(distance, 0.1f);
        SetCameraPosition();
        SetFov();
    }
    public void WheelDown() {
        distance += 0.2f;
        distance = MathF.Min(distance, maxDistance);
        SetCameraPosition();
        SetFov();
    }
    /// <summary>
    /// 单人机位
    /// </summary>
    /// <param name="character">角色</param>
    /// <param name="height">相机高度</param>
    /// <param name="distance">相机距离角色中心点的距离</param>
    public void LookAtCharacter(Node3D character, float height, float distance) {
        camera.GlobalPosition = character.GlobalPosition + Vector3.Up * height - GetDirection(character.Rotation + new Vector3(0, 0.5f * MathF.PI, 0)) * distance;
        camera.GlobalRotation = new Vector3(0, character.Rotation.Y - MathF.PI, 0);
    }
    public void LookAtCharacter(PlotCharacter character, float height, float distance) {
        LookAtCharacter(character.GetCharacterNode(), height, distance);
    }
    public void MoveCamera(Vector3 globalPosition) {
        camera.Position = globalPosition;
    }
    /// <summary>
    /// 把方向旋转转换为方向向量
    /// </summary>
    /// <param name="rotation">方向旋转的向量</param>
    /// <returns>方向向量</returns>
    public static Vector3 GetDirection(Vector3 rotation) {
        return new Vector3(MathF.Cos(rotation.Y), 0, MathF.Sin(rotation.Y));
    }
}
