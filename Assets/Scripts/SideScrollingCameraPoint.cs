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

    // Properties
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
            focus = P + rot * focus;
            return focus;
        }
    }
    public Vector3 PlayerTargetPosition => FocusPoint + Zone.transform.rotation * offset;
    SideScrollingZone zone = null;
    SideScrollingZone Zone
    {
        get
        {
            if (zone == null)
            {
                zone = GetComponentInParent<SideScrollingZone>();
            }
            return zone;
        }
    }
    Vector3 P => transform.position;

    // Debug
    GameplaySystemSetting Setting => GameplaySystem.Setting;
    void OnDrawGizmos()
    {
        if (!Camera.main) return;
        if (Zone == null) return;

        float y = Mathf.Tan(fov * Mathf.PI / 360f);
        float x = y * Camera.main.aspect;

        Vector3 topLeft = transform.localRotation * new Vector3(-x, y, 1);
        Vector3 botLeft = transform.localRotation * new Vector3(-x, -y, 1);
        topLeft *= Depth / topLeft.z;
        botLeft *= Depth / botLeft.z;
        Vector3 topRight = new Vector3(-topLeft.x, topLeft.y, Depth);
        Vector3 botRight = new Vector3(-botLeft.x, botLeft.y, Depth);

        var rot = Zone.transform.rotation;
        topLeft = P + rot * topLeft;
        botLeft = P + rot * botLeft;
        topRight = P + rot * topRight;
        botRight = P + rot * botRight;

        if (transform.localPosition.z > 0) Gizmos.color = Color.red;
        else Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        Gizmos.DrawWireMesh(Setting.characterMesh, PlayerTargetPosition + Setting.playerFocusHeight * Vector3.down, Zone.transform.rotation);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(topLeft, botLeft);
        Gizmos.DrawLine(topRight, botRight);
        if (transform.localPosition.z > 0)
        {
            Gizmos.DrawLine(topLeft, botRight);
            Gizmos.DrawLine(topRight, botLeft);
        }
        else
        {
            Gizmos.DrawLine(topLeft, P);
            Gizmos.DrawLine(botLeft, P);
            Gizmos.DrawLine(topRight, P);
            Gizmos.DrawLine(botRight, P);
        }
    }
}
