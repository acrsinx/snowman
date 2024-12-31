using System;
using Godot;

public class CameraManager : object {
	private Camera3D camera;
    public RayCast3D cameraRay;
    public static Vector3 cameraVector = new(0.31f, 0, 1);
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
    private float distance = 5.0f;
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
    public void SetCameraPosition() {
        camera.Position = cameraVector*distance;
    }
	public void UpdateCamera(float fDelta, Node3D player) {
		if (distance > 5) {
			distance = 5;
			SetCameraPosition();
		} else if (distance < 0) {
			distance = 1;
			SetCameraPosition();
		}
		if (IsCameraTouching()) {
			camera.Position = Vector3.Zero;
			while (camera.Position.Y < 5) {
				camera.Position += cameraVector*0.2f;
				if (IsCameraTouching()) {
					camera.Position -= cameraVector*0.2f;
					distance = camera.Position.Z;
					break;
				}
			}
		} else if (distance < 5) {
			float record = distance;
			distance += fDelta;
			distance = Math.Min(distance, 5);
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
		distance = MathF.Min(distance, 5);
		SetCameraPosition();
		SetFov();
	}
	public void MoveCamera(Vector3 globalPosition) {
		camera.Position = globalPosition;
	}
}