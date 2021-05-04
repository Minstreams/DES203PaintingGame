namespace GameSystem
{
    /// <summary>
    /// 游戏场景枚举，定义了游戏中的所有场景
    /// All scenes in this game
    /// </summary>
    public enum SceneCode
    {
        logo,
        startMenu,
        museum,
        painting1,
        CherryBlossom,
        MazeLevel,
        ClimbLevel,
        CastleLevel,
        GameOver,
    }
    // EnumMap Class Definition (必须以Map作为名称结尾)
    [System.Serializable]
    public class SceneCodeMap : EnumMap<SceneCode, string> { }
}