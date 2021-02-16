using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// 键鼠输入按键种类
    /// </summary>
    public enum InputKey
    {
        Up,
        Down,
        Left,
        Right,
        Action,
    }

    [System.Serializable]
    public class InputKeyMap : EnumMap<InputKey, KeyCode> { }
}