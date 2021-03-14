using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// To perform a fade-in effect on an ui element.
    /// </summary>
    [AddComponentMenu("|Operator/UIFading")]
    public class UIFading : MonoBehaviour
    {
        [MinsHeader("UI Fading", SummaryType.TitleYellow, 0)]
        [MinsHeader("To perform a fade-in or fade-out effect on an ui element.", SummaryType.CommentCenter, 1)]
        [LabelRange(-1, 1)] public float offsetX;
        [LabelRange(-1, 1)] public float offsetY;
        [LabelRange(-1, 1)] public float offsetZ;
        [Label] public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label] public float time = 1;
        [Label] public bool hasColorEffect;
        [ConditionalShow("hasColorEffect")] public Color inColor = Color.clear;
        [ConditionalShow("hasColorEffect")] public Color normalColor = Color.black;
        [ConditionalShow("hasColorEffect")] public Color outColor = Color.clear;
        [ConditionalShow("hasColorEffect")] public ColorEvent colorOutput;

        float scale;
        Vector3 offset;
        Vector3 targetPos;

        void Start()
        {
            scale = transform.lossyScale.x;
            offset = new Vector3(offsetX * Screen.width * scale, offsetY * Screen.height * scale, offsetZ * Screen.height * scale);
            targetPos = transform.position;
        }

        [Label] public SimpleEvent onStart;
        [Label] public SimpleEvent onFinish;

        // Input
        [ContextMenu("Invoke")]
        public void Invoke()
        {
            StopAllCoroutines();
            var fout = GetComponent<RectTransformFadeout>();
            if (fout != null) fout.StopAllCoroutines();

            colorOutput?.Invoke(normalColor);
            gameObject.SetActive(true);

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
                if (hasColorEffect) colorOutput?.Invoke(Color.Lerp(inColor, normalColor, t));
                timer += Time.deltaTime / time;
                yield return 0;
            }
            transform.position = targetPos;
            if (hasColorEffect) colorOutput?.Invoke(normalColor);
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