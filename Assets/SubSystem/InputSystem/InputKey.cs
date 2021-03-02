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
        Jump,
        Attack,
        Action,
        Pause,
    }

    [System.Serializable]
    public class InputKeyMap : EnumMap<InputKey, KeyCode> { }
}