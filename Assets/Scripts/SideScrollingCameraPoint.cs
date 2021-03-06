using UnityEngine;
using GameSystem;
using GameSystem.Setting;

public class SideScrollingCameraPoint : MonoBehaviour
{
    [Label, SerializeField] Vector2 offset;
    [LabelRange(15, 90)] public float fov = 60;
    GameplaySystemSetting Setting => GameplaySystem.Setting;

    public Vector2 Offset
    {
        get => offset;
        set
        {
            float theta = fov * Mathf.PI / 360f;
            float height = -transform.localPosition.z * Mathf.Tan(theta);
            float width = height * Camera.main.aspect;
            if (value.y > height) value.y = height;
            if (value.y < -height) value.y = -height;
            if (value.x > width) value.x = width;
            if (value.x < -width) value.x = -width;
            offset = value;
        }
    }

    private void Start()
    {
        //SmartCamera.cameraPoints.Add(new SmartCamera.SmartCameraPoint((Vector2)transform.position + offset, -offset, transform.position.z, fov));
    }
    public Vector3 GetPlayerTargetPosition()
    {
        var focusPoint = transform.position - transform.forward * transform.localPosition.z;
        return focusPoint + transform.rotation * offset;
    }

    private void OnDrawGizmos()
    {
        if (!Camera.main) return;
        SideScrollingZone zone = GetComponentInParent<SideScrollingZone>();
        if (zone == null) return;
        float theta = fov * Mathf.PI / 360f;
        float height = -transform.localPosition.z * Mathf.Tan(theta);
        float width = height * Camera.main.aspect;

        if (transform.localPosition.z > 0) Gizmos.color = Color.red;
        else Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        var focusPoint = transform.position - transform.forward * transform.localPosition.z;
        var playerPoint = focusPoint + transform.rotation * offset;
        Gizmos.DrawWireMesh(Setting.characterMesh, playerPoint + Setting.playerFocusHeight * Vector3.down, transform.rotation);

        var sizeY = height * Vector3.up;
        var sizeX = width * transform.right;
        Gizmos.DrawLine(focusPoint - sizeX - sizeY, focusPoint + sizeX - sizeY);
        Gizmos.DrawLine(focusPoint - sizeX - sizeY, focusPoint - sizeX + sizeY);
        Gizmos.DrawLine(focusPoint + sizeX + sizeY, focusPoint + sizeX - sizeY);
        Gizmos.DrawLine(focusPoint + sizeX + sizeY, focusPoint - sizeX + sizeY);

        if (transform.localPosition.z > 0)
        {
            Gizmos.DrawLine(focusPoint - sizeX - sizeY, focusPoint);
            Gizmos.DrawLine(focusPoint - sizeX + sizeY, focusPoint);
            Gizmos.DrawLine(focusPoint + sizeX - sizeY, focusPoint);
            Gizmos.DrawLine(focusPoint + sizeX + sizeY, focusPoint);
        }
        else
        {
            Gizmos.DrawLine(focusPoint - sizeX - sizeY, transform.position);
            Gizmos.DrawLine(focusPoint - sizeX + sizeY, transform.position);
            Gizmos.DrawLine(focusPoint + sizeX - sizeY, transform.position);
            Gizmos.DrawLine(focusPoint + sizeX + sizeY, transform.position);
        }
    }
}
