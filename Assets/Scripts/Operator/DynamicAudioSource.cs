using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// Control the pitch & volume of the audio dynamically
    /// </summary>
    [AddComponentMenu("|Operator/DynamicAudioSource")]
    public class DynamicAudioSource : MonoBehaviour
    {
        [MinsHeader("DynamicAudioSource", SummaryType.TitleYellow, 0)]
        [MinsHeader("Control the pitch & volume of the audio dynamically", SummaryType.CommentCenter, 1)]

        [Label("Audio Source")] public AudioSource aus;

        [Label] public Vector2 inputRange = Vector2.up;
        [Label] public Vector2 pitchRange = Vector2.one;
        [Label] public Vector2 volumeRange = Vector2.up;

        [Label] public AudioClip[] clips;

        public void Play(float input)
        {
            float t = Mathf.InverseLerp(inputRange.x, inputRange.y, input);
            aus.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, t);
            aus.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, t);
            if (clips != null && clips.Length > 0) aus.clip = clips[Random.Range(0, clips.Length)];
            aus.Play();
        }
        public void Play()
        {
            if (clips != null && clips.Length > 0) aus.clip = clips[Random.Range(0, clips.Length)];
            aus.Play();
        }

        public void Set(float input)
        {
            float t = Mathf.InverseLerp(inputRange.x, inputRange.y, input);
            aus.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, t);
            aus.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, t);
        }
    }
}