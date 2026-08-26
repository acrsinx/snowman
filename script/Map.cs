using Godot;
public class Map: object {
    public static readonly float[] mapSizes = {
        17.0f,
        29.0f
    };
    /// <summary>
    /// 获取全局坐标到小地图坐标的缩放因子y，单位为像素/米
    /// </summary>
    /// <param name="map">小地图</param>
    /// <param name="scene">场景编号</param>
    /// <returns>缩放因子y</returns>
    public static float GetFactorY(Sprite2D map, int scene) {
        // factor(pixel/meter) = mapPngSize(pixel) * scale / mapSize(meter)
        return map.Texture.GetHeight() * map.Scale.Y / mapSizes[scene];
    }
    /// <summary>
    /// 获取全局坐标到小地图坐标的缩放因子x，单位为像素/米
    /// </summary>
    /// <param name="map">小地图</param>
    /// <param name="scene">场景编号</param>
    /// <returns>缩放因子x</returns>
    public static float GetFactorX(Sprite2D map, int scene) {
        // factor(pixel/meter) = mapPngSize(pixel) * scale / mapSize(meter)
        return map.Texture.GetWidth() * map.Scale.X / mapSizes[scene];
    }
    /// <summary>
    /// 从全局坐标获取小地图坐标
    /// </summary>
    /// <param name="playerPosition">玩家坐标</param>
    /// <param name="globalPosition">要转换的坐标</param>
    /// <param name="panel">小地图显示框</param>
    /// <param name="map">小地图</param>
    /// <param name="scene">场景编号</param>
    /// <returns>小地图坐标</returns>
    public static Vector2 GlobalPositionToMapPosition(Vector3 playerPosition, Vector3 globalPosition, Panel panel, Sprite2D map, int scene) {
        Vector2 player = new(playerPosition.X, playerPosition.Z);
        Vector2 that = new(globalPosition.X, globalPosition.Z);
        Vector2 delta = that - player;
        delta = new(delta.X * GetFactorX(map, scene), delta.Y * GetFactorY(map, scene));
        return delta + panel.GetRect().Size * 0.5f;
    }
    /// <summary>
    /// 从全局坐标获取小地图坐标
    /// </summary>
    /// <param name="player">玩家</param>
    /// <param name="globalPosition">要转换的坐标</param>
    /// <returns>小地图坐标</returns>
    public static Vector2 GlobalPositionToMapPosition(Player player, Vector3 globalPosition) {
        return GlobalPositionToMapPosition(player.character.GlobalPosition, globalPosition, player.ui.panel, player.ui.map, player.ui.currentScene);
    }
}
