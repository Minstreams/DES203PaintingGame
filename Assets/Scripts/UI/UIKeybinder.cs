using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class UIKeybinder : Button
    {
        [Label] public bool isMainKey = true;
        [Label] public InputKey key;
        [Label] public Text label;

        protected override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
            onClick.AddListener(() =>
            {
                UISettingPanel.CurrentKeybinder = this;
                Select();
            });
            label.text = (isMainKey ? InputSystem.Setting.MainKeys : InputSystem.Setting.SideKeys)[key].ToString();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (UISettingPanel.CurrentKeybinder == this) UISettingPanel.CurrentKeybinder = null;
        }

        public override void OnPointerExit(PointerEventData eventData) { }
        public override void OnPointerEnter(PointerEventData eventData) { }

        public void SetKey(KeyCode code)
        {
            if (isMainKey)
            {
                InputSystem.Setting.MainKeys[key] = code;
            }
            else
            {
                InputSystem.Setting.SideKeys[key] = code;
            }
            label.text = code.ToString();
            DoStateTransition(SelectionState.Normal, false);
            UISettingPanel.CurrentKeybinder = null;
        }
    }
}