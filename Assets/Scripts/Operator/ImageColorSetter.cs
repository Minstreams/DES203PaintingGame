using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Operator
{
    [AddComponentMenu("|Operator/ImageColorSetter")]
    public class ImageColorSetter : MonoBehaviour
    {
        [MinsHeader("Image Color Setter", SummaryType.TitleYellow, 0)]

        // Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label] public Image target;

        // Input
        public void Set(Color color)
        {
            target.color = color;
        }
    }
}