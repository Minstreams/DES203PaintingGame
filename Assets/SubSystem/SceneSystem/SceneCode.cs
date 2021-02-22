namespace GameSystem
{
    /// <summary>
    /// 游戏场景枚举，定义了游戏中的所有场景
    /// </summary>
    public enum SceneCode
    {
        logo,
        startMenu,
        enteringMuseum,
        museum,
        enteringPainting,
        exitingPainting,
        painting1,
        painting2
    }
    // EnumMap Class Definition (必须以Map作为名称结尾)
    [System.Serializable]
    public class SceneCodeMap : EnumMap<SceneCode, string> { }
}