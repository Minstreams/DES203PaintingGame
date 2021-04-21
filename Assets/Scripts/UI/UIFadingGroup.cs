using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{
    /// <summary>
    /// To control a group of UI fading operators.
    /// </summary>
    [AddComponentMenu("|UI/UIFadingGroup")]
    public class UIFadingGroup : UIFadable
    {
        [MinsHeader("UIFadingGroup", SummaryType.TitleYellow, 0)]
        [MinsHeader("To control a group of UI fading units.", SummaryType.CommentCenter, 1)]
        [Label("Target ", true)] public UIFadable[] targets;
        [MinsHeader("Events")]
        [Label] public SimpleEvent onFadein;
        [Label] public SimpleEvent onFadeout;

        bool active;
        [ContextMenu("Set Targets")]
        void SetTargets()
        {
            List<UIFadable> res = new List<UIFadable>();
            GetComponentsInChildren<UIFadable>(true, res);
            res.Remove(this);
            targets = res.ToArray();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        // Input
        [ContextMenu("Fadein")]
        public override void Fadein()
        {
            gameObject.SetActive(true);
            foreach (var f in targets) f.Fadein();
            active = true;
            onFadein?.Invoke();
        }
        [ContextMenu("Fadeout")]
        public override void Fadeout()
        {
            foreach (var f in targets) f.Fadeout();
            active = false;
            onFadeout?.Invoke();
        }
        [ContextMenu("Toggle")]
        public void Toggle()
        {
            if (active) Fadeout();
            else Fadein();
        }
        [ContextMenu("Hide")]
        public override void Hide()
        {
            gameObject.SetActive(true);
            foreach (var f in targets) f.Hide();
            active = false;
        }
    }
}