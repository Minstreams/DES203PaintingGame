using UnityEngine;
using GameSystem;
using GameSystem.Setting;

public class SideScrollingCameraPoint : MonoBehaviour
{
    // Panel
    [Label, SerializeField] Vector2 offset;
    [LabelRange(15, 90)] public float fov = 60;

    // Fields
    [System.NonSerialized] public float weight = 0;
    SideScrollingZone zone = null;

    // Properties
    SideScrollingZone Zone => zone == null ? zone = GetComponentInParent<SideScrollingZone>() : zone;

    public float Depth => -transform.localPosition.z;
    public Vector2 Offset
    {
        get => offset;
        set
        {
            var rot = Zone.transform.rotation;
            var focus = transform.localRotation * Vector3.forward;
            focus *= Depth / focus.z;
            var target = focus + (Vector3)value;
            target = Quaternion.Inverse(transform.localRotation) * target;
            target *= Depth / target.z;

            float height = Depth * Mathf.Tan(fov * Mathf.PI / 360f);
            float width = height * Camera.main.aspect;

            if (target.y > height) target.y = height;
            if (target.y < -height) target.y = -height;
            if (target.x > width) target.x = width;
            if (target.x < -width) target.x = -width;

            target = transform.localRotation * target;
            target *= Depth / target.z;
            target -= focus;

            offset = target;
        }
    }
    public Vector3 RelativePosition
    {
        get
        {
            var rot = Zone.transform.rotation;
            var focus = transform.localRotation * Vector3.forward;
            focus *= Depth / focus.z;
            focus += (Vector3)offset;
            return Quaternion.Inverse(transform.localRotation) * (-focus);
        }
    }
    public Vector3 FocusPoint
    {
        get
        {
            var rot = Zone.transform.rotation;
            var focus = transform.localRotation * Vector3.forward;
            focus *= Depth / focus.z;
            focus = transform.position + rot * focus;
            return focus;
        }
    }
    public Vector3 PlayerTargetPosition => FocusPoint + Zone.transform.rotation * offset;
}
