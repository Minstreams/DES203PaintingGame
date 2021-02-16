using UnityEngine;
using UnityEngine.Events;

namespace GameSystem
{
    #region 事件类型定义 (必须以Event作为名称结尾)
    [System.Serializable]
    public class SimpleEvent : UnityEvent { }
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    [System.Serializable]
    public class Vec2Event : UnityEvent<Vector2> { }
    [System.Serializable]
    public class Vec3Event : UnityEvent<Vector3> { }
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }
    [System.Serializable]
    public class ColorEvent : UnityEvent<Color> { }
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    #endregion
}