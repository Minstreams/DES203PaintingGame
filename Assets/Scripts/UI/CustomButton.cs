using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{

    [AddComponentMenu("|UI/CustomButton")]
    public class CustomButton : UIBase
    {
        [MinsHeader("Make a button on GUI to trigger an event.", SummaryType.CommentCenter, -1)]
        [Label] public string buttonText;

        public SimpleEvent output;
        protected override void OnUI()
        {
            if (GUILayout.Button(buttonText)) output?.Invoke();
        }
        protected override void Reset()
        {
            base.Reset();
            SetStyle(ref boxStyle, "label");
        }
    }
}