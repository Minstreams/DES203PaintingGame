using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;
using UnityEngine.Events;

namespace GameSystem.UI
{
    public class UISettingPanel : UIFadingGroup
    {
        InputSystemSetting ControlSetting => InputSystem.Setting;

        [MinsHeader("Settings", SummaryType.Title, -1), Separator]
        [MinsHeader("Control Setting")]
        [Label(true)] public UISliderHandler handlerMouseSensitivityX;
        [Label(true)] public UISliderHandler handlerMouseSensitivityY;
        [Label(true)] public UIToggleHandler handlerInvertY;

        void Start()
        {
            handlerMouseSensitivityX?.Bind(ControlSetting.mouseSensitivity.x, val => ControlSetting.mouseSensitivity.x = val);
            handlerMouseSensitivityY?.Bind(ControlSetting.mouseSensitivity.y, val => ControlSetting.mouseSensitivity.y = val);
            handlerInvertY?.Bind(ControlSetting.mouseInvertY, val => ControlSetting.mouseInvertY = val);
        }


        public static UIKeybinder CurrentKeybinder { get; set; } = null;

        void OnGUI()
        {
            if (CurrentKeybinder != null && Event.current.type == EventType.KeyDown)
            {
                var code = Event.current.keyCode;
                //
                if (code != KeyCode.None)
                {
                    CurrentKeybinder.SetKey(code);
                }
            }
        }

        public void Return()
        {
            transform.parent.GetComponent<UIFadingSwitcher>().SwitchTo(0);
        }
    }
}