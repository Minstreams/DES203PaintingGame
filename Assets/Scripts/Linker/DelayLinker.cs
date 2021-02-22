using UnityEngine;

namespace GameSystem.Linker
{
    [AddComponentMenu("|Linker/DelayLinker")]
    public class DelayLinker : MonoBehaviour
    {
        [MinsHeader("Delay Linker", SummaryType.TitleCyan, 0)]

        // Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label] public float delay = 0.5f;
        [Label] public bool invokeOnStart;

        // Output
        [MinsHeader("Output", SummaryType.Header, 3)]
        public SimpleEvent output;

        // Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            Invoke(delay);
        }
        public void Invoke(float delay) => Invoke(nameof(DoInvoke), delay);
        void DoInvoke() => output?.Invoke();


        void Start()
        {
            if (invokeOnStart) Invoke();
        }
    }
}