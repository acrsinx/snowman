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
    public bool playerSetZoom = false;
    public int posesAnimationTime;
    private enum PushState {
        timeSet,
        pushed1,
        playing,
        none
    };
    private PushState pushState = PushState.none;
    private Vector3 markerPosition1;
    private Vector3 markerRotation1;
    private Vector3 cameraPosition1;
    private Vector3 cameraRotation1;
    private Vector3 markerPosition2;
    private Vector3 markerRotation2;
    private Vector3 cameraPosition2;
    private Vector3 cameraRotation2;
    private long animationStartTime;
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
    public void UpdateCameraWhenMoving() {
        // 回正视场角
        WheelDown();
        // 回正相机角度
        ResetCameraRotation();
        // 玩家移动了，重设该值
        playerSetZoom = false;
    }
    /// <summary>
    /// 旋转视角
    /// </summary>
    /// <param name="mouseMove">鼠标位移</param>
    public void UpdateCameraWhenTurning(Vector2 mouseMove) {
        if (!playerSetZoom) { // 玩家刚刚没设置缩放
            WheelDown();
        }
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
        // 自动回正相机角度
        ResetCameraRotation();
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
    /// 处理相机动画
    /// </summary>
    public void PosesAnimation() {
        if (pushState != PushState.playing) {
            return;
        }
        // 混合系数
        float factor = (float)(player.ui.totalGameTime - animationStartTime) / posesAnimationTime;
        // 检测是否结束
        if (factor > 1) {
            factor = 1;
            pushState = PushState.none;
        }
        // 混合起始与结束
        cameraMarker.GlobalPosition = Tool.Mix(markerPosition1, markerPosition2, factor);
        cameraMarker.GlobalRotation = Tool.Mix(markerRotation1, markerRotation2, factor);
        camera.GlobalPosition = Tool.Mix(cameraPosition1, cameraPosition2, factor);
        camera.GlobalRotation = Tool.Mix(cameraRotation1, cameraRotation2, factor);
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
    /// 回正相机角度
    /// </summary>
    public void ResetCameraRotation() {
        if (cameraMarker.Rotation.X > CameraMarkerRotationMaxX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatToAngle(cameraMarker.Rotation.X, CameraMarkerRotationMaxX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (cameraMarker.Rotation.X < CameraMarkerRotationMinX) {
            cameraMarker.Rotation = new Vector3(Tool.FloatToAngle(cameraMarker.Rotation.X, CameraMarkerRotationMinX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
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
    public void SetPosesAnimationTime(int time) {
        if (pushState != PushState.none) {
            player.ui.Log("在不合适的时机设置相机运动时间");
        }
        if (time < 0) {
            player.ui.Log("相机运动时间不能小于0");
            return;
        }
        posesAnimationTime = time;
        pushState = PushState.timeSet;
    }
    public void PushCurrentCameraPose() {
        switch (pushState) {
            case PushState.timeSet: {
                markerPosition1 = cameraMarker.GlobalPosition;
                markerRotation1 = cameraMarker.GlobalRotation;
                cameraPosition1 = camera.GlobalPosition;
                cameraRotation1 = camera.GlobalRotation;
                pushState = PushState.pushed1;
                break;
            }
            case PushState.pushed1: {
                markerPosition2 = cameraMarker.GlobalPosition;
                markerRotation2 = cameraMarker.GlobalRotation;
                cameraPosition2 = camera.GlobalPosition;
                cameraRotation2 = camera.GlobalRotation;
                pushState = PushState.playing;
                animationStartTime = player.ui.totalGameTime;
                break;
            }
            case PushState.playing: {
                player.ui.Log("在相机动画时压入");
                return;
            }
            case PushState.none: {
                player.ui.Log("在写入相机动画时间前压入");
                return;
            }
        }
    }
    public void PauseCameraAnimation() {
        animationStartTime = player.ui.totalGameTime - posesAnimationTime;
        pushState = PushState.none;
    }
}
