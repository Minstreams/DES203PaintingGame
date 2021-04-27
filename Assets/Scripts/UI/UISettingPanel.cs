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
        [MinsHeader("Video Setting")]
        [Label(true)] public UIToggleHandler handlerFullScreen;
        [MinsHeader("Audio Setting")]
        [Label(true)] public UISliderHandler handlerMasterVolume;
        [Label(true)] public UISliderHandler handlerMusicVolume;
        [Label(true)] public UISliderHandler handlerSoundVolume;
        [MinsHeader("Control Setting")]
        [Label(true)] public UISliderHandler handlerMouseSensitivityX;
        [Label(true)] public UISliderHandler handlerMouseSensitivityY;
        [Label(true)] public UIToggleHandler handlerInvertY;

        void Start()
        {
            handlerFullScreen?.Bind(Screen.fullScreen == true, val =>
            {
                if (val)
                {
                    var res = Screen.resolutions[Screen.resolutions.Length - 1];
                    Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
            });

            handlerMasterVolume?.Bind(AudioSystem.GetVolume("VolumeMaster"), val => AudioSystem.SetVolume("VolumeMaster", val));
            handlerMusicVolume?.Bind(AudioSystem.GetVolume("VolumeMusic"), val => AudioSystem.SetVolume("VolumeMusic", val));
            handlerSoundVolume?.Bind(AudioSystem.GetVolume("VolumeSound"), val => AudioSystem.SetVolume("VolumeSound", val));

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