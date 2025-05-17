using Godot;
using System;
public class Tool: object {
    public delegate void Void();
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
    /// 用于将from平滑地移动到to，速度为speed，但不会超过to，返回新的from
    /// </summary>
    /// <param name="from">从</param>
    /// <param name="to">到</param>
    /// <param name="speed">速度（选填）</param>
    /// <returns>新的向量</returns>
    public static Vector3 Vector3To(Vector3 from, Vector3 to, float speed = 0.1f) {
        return new Vector3(Mathf.Lerp(from.X, to.X, speed), Mathf.Lerp(from.Y, to.Y, speed), Mathf.Lerp(from.Z, to.Z, speed));
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
    /// <summary>
    /// 混合
    /// </summary>
    /// <param name="a">甲</param>
    /// <param name="b">乙</param>
    /// <param name="factor">系数</param>
    /// <returns>混合结果</returns>
    public static Vector3 Mix(Vector3 a, Vector3 b, float factor) {
        return a * (1 - factor) + b * factor;
    }
    /// <summary>
    /// 将文件大小转换为字符串
    /// </summary>
    /// <param name="size">文件大小</param>
    /// <returns>字符串</returns>
    public static string Size(int size) {
        if (size < 1024) {
            return size + " B";
        }
        if (size < 1024 * 1024) {
            return (size / 1024.0f).ToString("F2") + " KB";
        }
        if (size < 1024 * 1024 * 1024) {
            return (size / 1024.0f / 1024.0f).ToString("F2") + " MB";
        }
        return (size / 1024.0f / 1024.0f / 1024.0f).ToString("F2") + " GB";
    }
    /// <summary>
    /// 打印资产文件
    /// </summary>
    /// <param name="path">路径</param>
    public static void PrintAssets(string path = "res://") {
        DirAccess dir = DirAccess.Open(path);
        if (dir == null) {
            return;
        }
        dir.ListDirBegin();
        // 获取子文件或文件夹
        for (string file = " "; file != ""; file = dir.GetNext()) {
            if (file == " ") {
                continue;
            }
            if (file.Contains('.')) {
                if (!FileAccess.FileExists(path + file)) {
                    GD.Print(path + file + "（不存在）");
                    continue;
                }
                // 获取文件大小
                int size = FileAccess.GetFileAsBytes(path + file).Length;
                GD.Print(path + file + " " + Size(size));
                continue;
            }
            PrintAssets(path + file + "/");
        }
        dir.ListDirEnd();
    }
}
