using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// To perform a fade-in effect on an ui element.
    /// </summary>
    [AddComponentMenu("|Operator/RectTransformFadein")]
    public class RectTransformFadein : MonoBehaviour
    {
        [MinsHeader("RectTransformFadein", SummaryType.TitleYellow, 0)]
        [MinsHeader("To perform a fade-in effect on an ui element.", SummaryType.CommentCenter, 1)]
        [LabelRange(-1, 1)] public float offsetX;
        [LabelRange(-1, 1)] public float offsetY;
        [LabelRange(-1, 1)] public float offsetZ;
        [Label] public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label] public float time = 1;
        [Label] public bool invokeOnEnable;
        [Label] public bool hasColorEffect;
        [ConditionalShow("hasColorEffect")] public Color startColor = Color.clear;
        [ConditionalShow("hasColorEffect")] public Color endColor = Color.black;
        [ConditionalShow("hasColorEffect")] public ColorEvent colorOutput;

        float scale;
        Vector3 offset;
        Vector3 targetPos;


        bool initialized;
        void Start()
        {
            initialized = true;
            scale = transform.lossyScale.x;
            offset = new Vector3(offsetX * Screen.width * scale, offsetY * Screen.height * scale, offsetZ * Screen.height * scale);
            targetPos = transform.position;
            if (invokeOnEnable) Invoke();
        }
        void OnEnable()
        {
            if (initialized && invokeOnEnable) Invoke();
        }

        public SimpleEvent onStart;
        public SimpleEvent onFinish;

        // Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            if (!isActiveAndEnabled) return;
            StopAllCoroutines();
            var fout = GetComponent<RectTransformFadeout>();
            if (fout != null) fout.StopAllCoroutines();
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
                transform.position = targetPos + (1 - t) * offset;
                if (hasColorEffect) colorOutput?.Invoke(Color.Lerp(startColor, endColor, t));
                timer += Time.deltaTime / time;
                yield return 0;
            }
            transform.position = targetPos;
            if (hasColorEffect) colorOutput?.Invoke(endColor);
            onFinish?.Invoke();
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
                pos = targetPos;
            }
            var center = pos + centerOffset;
            var target = center + offset;
            Gizmos.color = Color.cyan;
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