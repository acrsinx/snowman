using Godot;
using System;
public class Tool: object {
    public static readonly Random random = new();
    /// <summary>
    /// 随机单精度浮点数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机数</returns>
    public static float RandomFloat(float min, float max) {
        return random.NextSingle() * (max - min) + min;
    }
    /// <summary>
    /// 随机三维向量
    /// </summary>
    /// <param name="range">向量的范围(x, y, z), x ∈ [-range.X, range.X), y ∈ [-range.Y, range.Y), z ∈ [-range.Z, range.Z)</param>
    /// <returns>随机向量</returns>
    public static Vector3 RandomVector3(Vector3 range) {
        return new Vector3(RandomFloat(-1, 1) * range.X, RandomFloat(-1, 1) * range.Y, RandomFloat(-1, 1) * range.Z);
    }
    /// <summary>
    /// 用于将from平滑地移动到to，速度为speed，但不会超过to，返回新的from，from和to都是弧度
    /// </summary>
    /// <param name="from">从</param>
    /// <param name="to">到</param>
    /// <param name="speed">速度（选填）</param>
    /// <returns>新的值</returns>
    public static float FloatTo(float from, float to, float speed = 0.1f) {
        float PI2 = 2.0f * MathF.PI;
        float newTo = to - MathF.Floor(to / PI2) * PI2;
        newTo += MathF.Floor(from / PI2) * PI2;
        if (MathF.Abs(newTo + PI2 - from) < MathF.Abs(newTo - from)) {
            newTo += PI2;
        }
        if (MathF.Abs(newTo - PI2 - from) < MathF.Abs(newTo - from)) {
            newTo -= PI2;
        }
        if (newTo > from) {
            return MathF.Min(from + speed, newTo);
        } else {
            return MathF.Max(from - speed, newTo);
        }
    }
    /// <summary>
    /// 位置是否在指定范围内
    /// </summary>
    /// <param name="area">范围</param>
    /// <param name="position">位置</param>
    /// <returns>是否在</returns>
    public static bool IsInArea(Control area, Vector2 position) {
        return area.GetGlobalRect().HasPoint(position);
    }
    /// <summary>
    /// 位置是否在指定范围内，不能处理旋转，缩放不均匀等情况
    /// </summary>
    /// <param name="area">范围</param>
    /// <param name="position">位置</param>
    /// <returns>是否在</returns>
    public static bool IsInArea(TouchScreenButton area, Vector2 position) {
        CircleShape2D shape = area.Shape as CircleShape2D;
        float Scale = area.Scale.X;
        float radius = shape.Radius * Scale;
        return position.DistanceTo(area.GlobalPosition + 0.5f * Scale * area.TextureNormal.GetSize()) < radius;
    }
}
