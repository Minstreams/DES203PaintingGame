using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// To control a group of UI fading operators.
    /// </summary>
    [AddComponentMenu("|Operator/UIFadingGroup")]
    public class UIFadingGroup : MonoBehaviour
    {
        [MinsHeader("UIFadingGroup", SummaryType.TitleYellow, 0)]
        [MinsHeader("To control a group of UI fading operators.", SummaryType.CommentCenter, 1)]
        [Label("Active on Start"), SerializeField] bool active;
        [Label] public bool overrideTargets;
        [ConditionalShow("overrideTargets", Label = "Target ")] public UIFading[] targets;
        [MinsHeader("Events")]
        [Label] public SimpleEvent onFadein;
        [Label] public SimpleEvent onFadeout;

        void Start()
        {
            if (!overrideTargets) targets = GetComponentsInChildren<UIFading>(true);
            if (active) Fadein();
            else foreach (var f in targets) f.gameObject.SetActive(false);
        }

        // Input
        [ContextMenu("Fadein")]
        public void Fadein()
        {
            foreach (var f in targets) f.Fadein();
            active = true;
            onFadein?.Invoke();
        }
        [ContextMenu("Fadeout")]
        public void Fadeout()
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
    }
}