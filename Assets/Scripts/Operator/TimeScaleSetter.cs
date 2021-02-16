using UnityEngine;

namespace GameSystem.Operator
{

    [AddComponentMenu("|Operator/TimeScaleSetter")]
    public class TimeScaleSetter : MonoBehaviour
    {
        [MinsHeader("TimeScaleSetter", SummaryType.TitleYellow, 0)]
        [MinsHeader("", SummaryType.CommentCenter, 1)]
        [LabelRange(0, 2)] public float timeScale = 1;
        void Update()
        {
            Time.timeScale = timeScale;
        }
    }
}