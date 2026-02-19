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
    /// 相机标志的最小旋转角度
    /// </summary>
    public const float CameraMarkerRotationMinX = -1.2f;
    /// <summary>
    /// 相机标志的最大旋转角度
    /// </summary>
    public const float CameraMarkerRotationMaxX = 0.5f;
    /// <summary>
    /// 相机标志的最小合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationSuitMinX = -0.5f;
    /// <summary>
    /// 相机标志的最大合适旋转角度
    /// </summary>
    public const float CameraMarkerRotationSuitMaxX = 0.1f;
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
        cameraMarker.Position = cameraShake.GetShakeOffset(Ui.totalGameTime) + CameraMarkerOrigin;
        camera.Position = cameraVector * distance;
        camera.Rotation = Vector3.Zero;
        SetFov();
    }
    /// <summary>
    /// 将相机移动到指定位置
    /// </summary>
    /// <param name="position">目标位置</param>
    public void SetCameraPositionAt(Vector3 position) {
        camera.GlobalPosition = position;
    }
    /// <summary>
    /// 将相机旋转到指定角度
    /// </summary>
    /// <param name="x">x 角度制</param>
    /// <param name="y">y 角度制</param>
    public void SetCameraRotation(float x, float y) {
        camera.GlobalRotation = new Vector3(Mathf.DegToRad(x), Mathf.DegToRad(y), 0);
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
        if (CameraMarkerRotationMinX > cameraMarker.Rotation.X) {
            cameraMarker.Rotation = new Vector3(CameraMarkerRotationMinX, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        } else if (CameraMarkerRotationMaxX < cameraMarker.Rotation.X) {
            cameraMarker.Rotation = new Vector3(CameraMarkerRotationMaxX, cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (player.ui.settingPanel.gameInformation.UiType == UiType.computer) {
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
        cameraMarker.Position = cameraShake.GetShakeOffset(Ui.totalGameTime) + CameraMarkerOrigin;
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
        float factor = (float)(Ui.totalGameTime - animationStartTime) / posesAnimationTime;
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
        // 前移相机，直到不碰为止
        while (true) {
            distance -= CameraZoomSpeed;
            SetCameraPosition();
            if (distance < minDistance) {
                Ui.Log("相机穿模，找不到合适的位置");
                return;
            }
            if (!IsCameraTouching()) {
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
        if (cameraMarker.Rotation.X > CameraMarkerRotationSuitMaxX) {
            cameraMarker.Rotation = new Vector3(Mathf.LerpAngle(cameraMarker.Rotation.X, CameraMarkerRotationSuitMaxX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
        }
        if (cameraMarker.Rotation.X < CameraMarkerRotationSuitMinX) {
            cameraMarker.Rotation = new Vector3(Mathf.LerpAngle(cameraMarker.Rotation.X, CameraMarkerRotationSuitMinX, 0.01f), cameraMarker.Rotation.Y, cameraMarker.Rotation.Z);
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
            Ui.Log("在不合适的时机设置相机运动时间");
        }
        if (time < 0) {
            Ui.Log("相机运动时间不能小于0");
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
                animationStartTime = Ui.totalGameTime;
                break;
            }
            case PushState.playing: {
                Ui.Log("在相机动画时压入");
                return;
            }
            case PushState.none: {
                Ui.Log("在写入相机动画时间前压入");
                return;
            }
        }
    }
    public void PauseCameraAnimation() {
        animationStartTime = Ui.totalGameTime - posesAnimationTime;
        pushState = PushState.none;
    }
}
