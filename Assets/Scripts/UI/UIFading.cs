using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{
    /// <summary>
    /// To perform a fade-in effect on an ui element.
    /// </summary>
    [AddComponentMenu("|UI/UIFading")]
    public class UIFading : UIFadable
    {
        [MinsHeader("UI Fading", SummaryType.TitleYellow, 0)]
        [MinsHeader("To perform a fade-in or fade-out effect on an ui element.", SummaryType.CommentCenter, 1)]
        [Label] public bool hasColorEffect;
        [ConditionalShow("hasColorEffect")] public Color normalColor = Color.black;
        [ConditionalShow("hasColorEffect")] public ColorEvent colorOutput;

        [MinsHeader("Fade-in")]
        [ConditionalShow("hasColorEffect")] public Color inColor = Color.clear;
        [Label(true)] public Vector3 inOffset;
        [Label] public AnimationCurve inCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label] public float inTime = 1;
        [Label] public SimpleEvent inStart;
        [Label] public SimpleEvent inFinish;

        [MinsHeader("Fade-out")]
        [ConditionalShow("hasColorEffect")] public Color outColor = Color.clear;
        [Label(true)] public Vector3 outOffset;
        [Label] public AnimationCurve outCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Label] public float outTime = 1;
        [Label] public SimpleEvent outStart;
        [Label] public SimpleEvent outFinish;

        Vector3 originPos;
        bool fadingIn;
        Vector3 inScreenOffset;
        bool fadingOut;
        Vector3 outScreenOffset;

        bool initialized;

        void Initialize()
        {
            originPos = transform.position;
            var scale = transform.lossyScale.x;
            inScreenOffset = new Vector3(inOffset.x * Screen.width * scale, inOffset.y * Screen.height * scale, inOffset.z * Screen.height * scale);
            outScreenOffset = new Vector3(outOffset.x * Screen.width * scale, outOffset.y * Screen.height * scale, outOffset.z * Screen.height * scale);
            initialized = true;
        }

        // Input
        [ContextMenu("Fadein")]
        public override void Fadein()
        {
            Fadein(inTime);
        }
        public void Fadein(float time)
        {
            if (!initialized) Initialize();

            StopAllCoroutines();
            if (fadingOut) OnFadedout();

            if (hasColorEffect) colorOutput?.Invoke(normalColor);
            gameObject.SetActive(true);

            fadingIn = true;
            inStart?.Invoke();
            StartCoroutine(fadein(time));
        }

        IEnumerator fadein(float time)
        {
            float timer = 0;
            while (timer < 1)
            {
                float t = inCurve.Evaluate(timer);
                transform.position = originPos + (1 - t) * inScreenOffset;
                if (hasColorEffect) colorOutput?.Invoke(Color.Lerp(inColor, normalColor, t));
                timer += Time.unscaledDeltaTime / time;
                yield return 0;
            }
            transform.position = originPos;
            if (hasColorEffect) colorOutput?.Invoke(normalColor);
            OnFadedin();
        }
        void OnFadedin()
        {
            inFinish?.Invoke();
            fadingIn = false;
        }

        [ContextMenu("Fadeout")]
        public override void Fadeout()
        {
            Fadeout(outTime);
        }
        public void Fadeout(float time)
        {
            if (!initialized) Initialize();

            StopAllCoroutines();
            if (fadingIn) OnFadedin();

            if (hasColorEffect) colorOutput?.Invoke(normalColor);
            gameObject.SetActive(true);

            fadingOut = true;
            outStart?.Invoke();
            StartCoroutine(fadeout(time));
        }
        IEnumerator fadeout(float time)
        {
            float timer = 0;
            while (timer < 1)
            {
                float t = outCurve.Evaluate(timer);
                transform.position = originPos + t * outScreenOffset;
                if (hasColorEffect) colorOutput?.Invoke(Color.Lerp(normalColor, outColor, t));
                timer += Time.unscaledDeltaTime / time;
                yield return 0;
            }
            transform.position = originPos;
            if (hasColorEffect) colorOutput?.Invoke(outColor);
            OnFadedout();
        }
        void OnFadedout()
        {
            outFinish?.Invoke();
            gameObject.SetActive(false);
            fadingOut = false;
        }

        [ContextMenu("Hide")]
        public override void Hide()
        {
            StopAllCoroutines();
            fadingIn = fadingOut = false;
            gameObject.SetActive(false);
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var c = Gizmos.color;

            var rect = GetComponent<RectTransform>().rect;
            var scale = transform.lossyScale.x;
            var size = new Vector3(rect.width, rect.height) * scale;
            var pos = transform.position;
            Vector3 centerOffset = (Vector2.one * 0.5f - GetComponent<RectTransform>().pivot) * rect.size * scale;

            var inScreenOffset = new Vector3(inOffset.x * Screen.width, inOffset.y * Screen.height, inOffset.z * Screen.height) * scale;
            var outScreenOffset = new Vector3(outOffset.x * Screen.width, outOffset.y * Screen.height, outOffset.z * Screen.height) * scale;

            if (UnityEditor.EditorApplication.isPlaying)
            {
                Gizmos.color = new Color(1, 0, 1);
                Gizmos.DrawWireCube(pos + centerOffset, size);
                Gizmos.DrawWireSphere(pos, 5 * scale);
                pos = originPos;
            }

            var center = pos + centerOffset;
            var inTarget = center + inScreenOffset;
            var outTarget = center + outScreenOffset;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center + inScreenOffset, size);

            Gizmos.color = new Color(1, 0.3f, 0);
            Gizmos.DrawWireCube(center + outScreenOffset, size);

            size *= 0.5f;
            Gizmos.color = new Color(0.5f, 0.5f, 1);

            Gizmos.DrawLine(pos, pos + inScreenOffset);
            Gizmos.DrawLine(center + size, inTarget + size);
            Gizmos.DrawLine(center - size, inTarget - size);

            Gizmos.DrawLine(pos, pos + outScreenOffset);
            Gizmos.DrawLine(center + size, outTarget + size);
            Gizmos.DrawLine(center - size, outTarget - size);

            size.x = -size.x;

            Gizmos.DrawLine(center + size, inTarget + size);
            Gizmos.DrawLine(center - size, inTarget - size);

            Gizmos.DrawLine(center + size, outTarget + size);
            Gizmos.DrawLine(center - size, outTarget - size);

            Gizmos.color = c;
        }
#endif
    }
}