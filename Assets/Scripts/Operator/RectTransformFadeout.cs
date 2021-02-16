using System.Collections;
using UnityEngine;

namespace GameSystem.Operator
{

    [AddComponentMenu("|Operator/RectTransformFadeout")]
    public class RectTransformFadeout : MonoBehaviour
    {
        [MinsHeader("RecTransformFadeout", SummaryType.TitleYellow, 0)]
        [MinsHeader("To perform a fade-out effect on an ui element.", SummaryType.CommentCenter, 1)]
        [LabelRange(-1, 1)] public float offsetX;
        [LabelRange(-1, 1)] public float offsetY;
        [LabelRange(-1, 1)] public float offsetZ;
        [Label] public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label] public float time = 1;
        [Label] public bool hasColorEffect;
        [ConditionalShow("hasColorEffect")] public Color startColor = Color.black;
        [ConditionalShow("hasColorEffect")] public Color endColor = Color.clear;
        [ConditionalShow("hasColorEffect")] public ColorEvent colorOutput;

        float scale;
        Vector3 offset;
        Vector3 originPos;

        public SimpleEvent onStart;
        public SimpleEvent onFinish;

        // Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            if (!isActiveAndEnabled) return;
            scale = transform.lossyScale.x;
            offset = new Vector3(offsetX * Screen.width * scale, offsetY * Screen.height * scale, offsetZ * Screen.height * scale);
            originPos = transform.position;
            StopAllCoroutines();
            var fin = GetComponent<RectTransformFadein>();
            if (fin != null) fin.StopAllCoroutines();
            StartCoroutine(invoke(time));
        }
        public void Invoke(float time)
        {
            StartCoroutine(invoke(time));
        }

        IEnumerator invoke(float time)
        {
            onStart?.Invoke();
            float timer = 0;
            while (timer < 1)
            {
                float t = curve.Evaluate(timer);
                transform.position = originPos + t * offset;
                if (hasColorEffect) colorOutput?.Invoke(Color.Lerp(startColor, endColor, t));
                timer += Time.deltaTime / time;
                yield return 0;
            }
            transform.position = originPos;
            if (hasColorEffect) colorOutput?.Invoke(endColor);
            onFinish?.Invoke();
            gameObject.SetActive(false);
            // preserve color
            if (hasColorEffect) colorOutput?.Invoke(startColor);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var c = Gizmos.color;
            var rect = GetComponent<RectTransform>().rect;
            var scale = transform.lossyScale.x;
            var offset = new Vector3(offsetX * Screen.width, offsetY * Screen.height, offsetZ * Screen.height) * scale;
            var size = new Vector3(rect.width, rect.height) * scale;
            var pos = transform.position;
            Vector3 centerOffset = (Vector2.one * 0.5f - GetComponent<RectTransform>().pivot) * rect.size * scale;
            if (UnityEditor.EditorApplication.isPlaying)
            {
                Gizmos.color = new Color(1, 0, 1);
                Gizmos.DrawWireCube(pos + centerOffset, size);
                Gizmos.DrawWireSphere(pos, 5 * scale);
                pos = originPos;
            }
            var center = pos + centerOffset;
            var target = center + offset;
            Gizmos.color = new Color(1, 0.3f, 0);
            Gizmos.DrawWireCube(center + offset, size);
            size *= 0.5f;
            Gizmos.color = new Color(0.5f, 0.5f, 1);
            Gizmos.DrawLine(pos, pos + offset);
            Gizmos.DrawLine(center + size, target + size);
            Gizmos.DrawLine(center - size, target - size);
            size.x = -size.x;
            Gizmos.DrawLine(center + size, target + size);
            Gizmos.DrawLine(center - size, target - size);
            Gizmos.color = c;
        }
#endif
    }
}