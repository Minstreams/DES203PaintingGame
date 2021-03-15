using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/ThreadholdFloatLinker")]
    public class ThreadholdFloatLinker : MonoBehaviour
    {
        [MinsHeader("Threadhold Float Linker", SummaryType.TitleCyan, 0)]

        // Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label] public float threadhold = 0.5f;

        float value = 0;

        // Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        [Label] public SimpleEvent onOverThreadhold;
        [Label] public SimpleEvent onBelowThreadhold;


        // Input
        public void Invoke(float val)
        {
            if (value < threadhold && val >= threadhold) onOverThreadhold?.Invoke();
            if (value < threadhold && val <= threadhold) onBelowThreadhold?.Invoke();
            value = val;
        }
        public void SetThreadhold(float val) => threadhold = val;
    }
}