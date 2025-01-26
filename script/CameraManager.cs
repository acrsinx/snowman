using System;
using Godot;

public class CameraManager : object {
	private Camera3D camera;
    public RayCast3D cameraRay;
    private static Vector3 cameraVector = new(0.31f, 0, 1);
    public Vector3[] checkList = {
        new(-0.2f, 0, 0),
        new(0.2f, 0, 0),
        new(0, -0.2f, 0),
        new(0, 0.2f, 0),
        new(0, 0, -0.2f),
        new(0, 0, 0.2f),
        new(-0.2f, -0.2f, 0),
        new(-0.2f, 0.2f, 0),
        new(0.2f, -0.2f, 0),
        new(0.2f, 0.2f, 0),
        new(-0.2f, 0, -0.2f),
        new(-0.2f, 0, 0.2f),
        new(0.2f, 0, -0.2f),
        new(0.2f, 0, 0.2f),
        new(0, -0.2f, -0.2f),
        new(0, -0.2f, 0.2f),
        new(0, 0.2f, -0.2f),
        new(0, 0.2f, 0.2f)
    };
    private const float maxDistance = 3.0f;
    private float distance = 3.0f;
    public CameraManager(Camera3D camera, RayCast3D cameraRay) {
		this.camera = camera;
		this.cameraRay = cameraRay;
        SetCameraPosition();
        SetFov();
    }
    private void SetFov() {
        float fov =  75 + (distance - 5) * 5;
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
    // 将相机位置与方向重置
    public void SetCameraPosition() {
        camera.Position = cameraVector*distance;
        camera.Rotation = Vector3.Zero;
    }
	public void UpdateCamera(float fDelta, Node3D player) {
		if (distance > maxDistance) {
			distance = maxDistance;
			SetCameraPosition();
		} else if (distance < 0) {
			distance = 1;
			SetCameraPosition();
		}
		if (IsCameraTouching()) {
			camera.Position = Vector3.Zero;
			while (camera.Position.Y < maxDistance) {
				camera.Position += cameraVector*0.2f;
				if (IsCameraTouching()) {
					camera.Position -= cameraVector*0.2f;
					distance = camera.Position.Z;
					break;
				}
			}
		} else if (distance < maxDistance) {
			float record = distance;
			distance += fDelta;
			distance = Math.Min(distance, maxDistance);
			SetCameraPosition();
			if (IsCameraTouching()) {
				distance = record;
				SetCameraPosition();
			}
		}
		SetFov();
		player.Visible = camera.Position.Z > 2;
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
    // 单人机位
	public void LookAtCharacter(Node3D character, float height, float distance) {
        camera.GlobalPosition = character.GlobalPosition + Vector3.Up * height - GetDirection(character.Rotation)*distance;
        camera.GlobalRotation = new Vector3(0, character.Rotation.Y-0.5f*MathF.PI, 0);
	}
	public void LookAtCharacter(PlotCharacter character, float height, float distance) {
        LookAtCharacter(character.character, height, distance);
	}
	public void MoveCamera(Vector3 globalPosition) {
		camera.Position = globalPosition;
	}
    // 把方向旋转转换为方向向量
    public static Vector3 GetDirection(Vector3 rotation) {
        return new Vector3(MathF.Cos(rotation.Y), 0, MathF.Sin(rotation.Y));
    }
}