using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 封装输入的系统
    /// </summary>
    public class InputSystem : SubSystem<InputSystemSetting>
    {
        #region 输入层 - Keyboard ===============================
        // API---------------------------------
        public static bool GetKey(InputKey input)
        {
            return Input.GetKey(Setting.MainKeys[input]) || Input.GetKey(Setting.SideKeys[input]);
        }
        public static bool GetKeyDown(InputKey input)
        {
            return Input.GetKeyDown(Setting.MainKeys[input]) || Input.GetKeyDown(Setting.SideKeys[input]);
        }
        public static bool GetKeyUp(InputKey input)
        {
            return Input.GetKeyUp(Setting.MainKeys[input]) || Input.GetKeyUp(Setting.SideKeys[input]);
        }
        #endregion
    }
}
