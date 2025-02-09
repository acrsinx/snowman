using System;
using Godot;
public class CameraManager: object {
    private readonly Camera3D camera;
    public readonly Marker3D cameraMarker;
    public readonly ShapeCast3D cameraCast;
    public readonly Player player;
    /// <summary>
    /// 相机标志的原位置
    /// </summary>
    public static readonly Vector3 CameraMarkerOrigin = new(0, 0.5f, 0);
    /// <summary>
    /// 相机标志的最小合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationMinX = -0.5f;
    /// <summary>
    /// 相机标志的最大合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationMaxX = 0.1f;
    /// <summary>
    /// 相机视角缩放速度
    /// </summary>
    public const float CameraZoomSpeed = 0.05f;
    public Shake cameraShake = new();
    private static readonly Vector3 cameraVector = new(0.31f, 0, 1);
    private const float minDistance = -0.15f;
    private const float minSuitableDistance = 0.5f;
    private const float maxDistance = 2.0f;
    private float distance = maxDistance;
    public CameraManager(Camera3D camera, ShapeCast3D cameraCast, Player player, Marker3D cameraMarker) {
        this.camera = camera;
        this.cameraCast = cameraCast;
        this.player = player;
        this.cameraMarker = cameraMarker;
        SetCameraPosition();
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
        cameraCast.ForceShapecastUpdate();
        if (!cameraCast.IsColliding()) {
            return false;
        }
        GodotObject obj = cameraCast.GetCollider(0);
        if (obj is Node node) {
            HaveCharacter haveCharacter = HaveCharacter.GetHaveCharacter(node);
            if (haveCharacter == null) {
                return true;
            }
            if (haveCharacter.GetCharacter().isPlayer) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 将相机位置与方向重置
    /// </summary>
    public void SetCameraPosition() {
        cameraMarker.Position = cameraShake.GetShakeOffset(player.ui.totalGameTime) + CameraMarkerOrigin;
        camera.Position = cameraVector * distance;
        camera.Rotation = Vector3.Zero;
        SetFov();
    }
    /// <summary>
    /// 玩家移动时回正相机
    /// </summary>
    /// <param name="fDelta">时间增量</param>
    public void UpdateCameraWhenMoving() {
        // 回正视场角
        WheelDown();
        // 回正相机角度
        if (cameraMarker.Rotation.X > CameraMarkerRotationMaxX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatTo(cameraMarker.Rotation.X, CameraMarkerRotationMaxX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (cameraMarker.Rotation.X < CameraMarkerRotationMinX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatTo(cameraMarker.Rotation.X, CameraMarkerRotationMinX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
    }
    /// <summary>
    /// 旋转视角
    /// </summary>
    /// <param name="mouseMove">鼠标位移</param>
    public void UpdateCameraWhenTurning(Vector2 mouseMove) {
        // 处理cameraMarker.Rotation
        cameraMarker.Rotation = new Vector3(cameraMarker.Rotation.X + mouseMove.Y, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        // 处理player.character.Rotation
        player.character.Rotation = new Vector3(player.character.Rotation.X, player.character.Rotation.Y + mouseMove.X, player.character.Rotation.Z);
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
        } else if (distance < minDistance) {
            distance = minDistance;
            SetCameraPosition();
        }
        if (IsCameraTouching()) { // 相机穿模了
            DealWithCameraTouch();
        }
        SetCameraPosition();
        // 相机震动
        cameraMarker.Position = cameraShake.GetShakeOffset(player.ui.totalGameTime) + CameraMarkerOrigin;
        player.character.character.Visible = distance > 0.9f;
    }
    /// <summary>
    /// 处理相机穿模
    /// </summary>
    private void DealWithCameraTouch() {
        // 前移相机
        // 相机不穿模次数
        int i = 0;
        while (true) {
            distance -= CameraZoomSpeed;
            SetCameraPosition();
            if (distance < minDistance) {
                player.ui.Log("相机穿模，找不到合适的位置");
                return;
            }
            if (!IsCameraTouching()) {
                i++;
            }
            if (i > 3) {
                return;
            }
        }
    }
    public void WheelUp() {
        if (distance < minSuitableDistance) {
            return;
        }
        distance -= CameraZoomSpeed;
        distance = MathF.Max(distance, minSuitableDistance);
        SetCameraPosition();
        if (IsCameraTouching()) {
            distance += CameraZoomSpeed;
            SetCameraPosition();
        }
    }
    public void WheelDown() {
        float record = distance;
        // 相机退三不碰，方可退一
        for (int i = 0; i < 3; i++) {
            distance += CameraZoomSpeed;
            if (distance > maxDistance) {
                distance = maxDistance;
                SetCameraPosition();
                return;
            }
            SetCameraPosition();
            if (IsCameraTouching()) {
                distance = record;
                SetCameraPosition();
                return;
            }
        }
        distance = record + CameraZoomSpeed;
        SetCameraPosition();
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
