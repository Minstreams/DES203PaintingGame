using UnityEditor;
using GameSystem;

[CustomPropertyDrawer(typeof(AudioCodeMap), true)]
public class AudioCodeMapDrawer : EnumMapDrawer<AudioCode> { }