using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "InputSystemSetting", menuName = "系统配置文件/InputSystemSetting")]
    public class InputSystemSetting : ScriptableObject
    {
        /// <summary>
        /// 主要按键
        /// </summary>
        [MinsHeader("InputSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("封装输入的系统", SummaryType.CommentCenter, -1)]

        [MinsHeader("KeyBoard", SummaryType.SubTitle), Space(16)]
        [MinsHeader("主要按键", SummaryType.Header, 1)]
        [Label] public InputKeyMap MainKeys;
        /// <summary>
        /// 次要按键
        /// </summary>
        [MinsHeader("次要按键", SummaryType.Header)]
        [Label] public InputKeyMap SideKeys;

        [MinsHeader("Mouse", SummaryType.SubTitle, -1), Space(16)]
        [MinsHeader("ThirdPersonalView")]
        [Label] public Vector2 mouseSensitivity;
        [Label] public bool mouseInvertY;

    }
}