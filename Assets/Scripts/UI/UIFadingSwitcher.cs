using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{
    [AddComponentMenu("|UI/UIFadingSwitcher")]
    public class UIFadingSwitcher : UIFadable
    {
        [MinsHeader("UIFadingSwitcher", SummaryType.TitleYellow, 0)]
        [MinsHeader("To switch between UI fading units.", SummaryType.CommentCenter, 1)]
        [Label("Default Active Id", true), SerializeField] int activeId;
        [Label("Target ", true), SerializeField] UIFadable[] targets;
        [MinsHeader("Events")]
        [Label] public SimpleEvent onFadein;
        [Label] public SimpleEvent onFadeout;

        void Start()
        {
            for (int i = 0; i < targets.Length; ++i)
            {
                if (i != activeId) targets[i].Hide();
            }
        }

        bool active;
        public void SwitchTo(int id)
        {
            if (id == activeId) return;
            Fadeout();
            activeId = id;
            Fadein();
        }
        public void SetActiveId(int id)
        {
            activeId = id;
        }
        public override void Fadein()
        {
            gameObject.SetActive(true);
            targets[activeId].Fadein();
            onFadein?.Invoke();
            active = true;
        }
        public override void Fadeout()
        {
            targets[activeId].Fadeout();
            onFadeout?.Invoke();
            active = false;
        }
        public override void Hide()
        {
            gameObject.SetActive(true);
            targets[activeId].Hide();
            active = false;
        }
        [ContextMenu("Toggle")]
        public void Toggle()
        {
            if (active) Fadeout();
            else Fadein();
        }
    }
}