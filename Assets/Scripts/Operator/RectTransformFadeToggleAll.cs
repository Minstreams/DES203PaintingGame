using UnityEngine;

namespace GameSystem.Operator
{

    [AddComponentMenu("|Operator/RectTransformFadeToggleAll")]
    public class RectTransformFadeToggleAll : MonoBehaviour
    {
        [MinsHeader("Fadeout Toggle All", SummaryType.TitleYellow, 0)]
        [MinsHeader("Control multiple RectTransformFadeout Components.", SummaryType.CommentCenter, 1)]
        [Label] public bool givenTargets;
        [ConditionalShow("givenTargets", Label = "Target ")] public RectTransformFadeout[] targets;

        // Input
        [ContextMenu("Fadeout")]
        public void Fadeout()
        {

            if (givenTargets)
            {
                foreach (var f in targets) f.Invoke();
            }
            else
            {
                var fadeouts = GetComponentsInChildren<RectTransformFadeout>(true);
                foreach (var f in fadeouts) f.Invoke();
            }
        }
        [ContextMenu("FadeIn")]
        public void FadeIn()
        {
            if (givenTargets)
            {
                foreach (var f in targets) f.gameObject.SetActive(true);
            }
            else
            {
                var fadeouts = GetComponentsInChildren<RectTransformFadeout>(true);
                foreach (var f in fadeouts) f.gameObject.SetActive(true);
            }
        }
    }
}