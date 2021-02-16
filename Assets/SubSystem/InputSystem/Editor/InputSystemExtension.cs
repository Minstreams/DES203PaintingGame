using UnityEditor;
using GameSystem;

[CustomPropertyDrawer(typeof(InputKeyMap), true)]
public class InputKeyMapDrawer : EnumMapDrawer<InputKey> { }
