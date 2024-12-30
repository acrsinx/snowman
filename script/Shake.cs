using Godot;

public partial class Shake : object {
    public bool isShaking = false;
    public Vector3 offset = Vector3.Zero;
    public Vector3 ShakeTarget = Vector3.Zero;
    public long shakeStartTime = 0;
    public long shakeDuration = 0;
    public Vector3 GetShakeOffset(long totalGameTime) {
        if (isShaking) {
            if (shakeStartTime + shakeDuration < totalGameTime) {
                isShaking = false;
                return Vector3.Zero;
            }
            offset = 0.1f * offset + 0.9f * ShakeTarget;
            if (System.Random.Shared.NextSingle() < 0.9f) {
                SetRandomTarget();
            }
            return offset;
        }
        return Vector3.Zero;
    }
    public void StartShake(long totalGameTime, long duration) {
        isShaking = true;
        shakeStartTime = totalGameTime;
        shakeDuration = duration;
        offset = Vector3.Zero;
        SetRandomTarget();
    }
    public void SetRandomTarget() {
        ShakeTarget = new Vector3(System.Random.Shared.NextSingle() * 0.2f - 0.1f, System.Random.Shared.NextSingle() * 0.2f - 0.1f, System.Random.Shared.NextSingle() * 0.2f - 0.1f);
    }
}