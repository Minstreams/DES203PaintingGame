using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.Setting;

/// <summary>
/// Smart camera tracking the player avatar
/// </summary>
public class CameraController : MonoBehaviour
{
    // Properties
    public HashSet<SideScrollingZone> ActiveSideScrollingZoneSet { get; } = new HashSet<SideScrollingZone>();
    public SideScrollingCameraPoint CameraPointLeft { get; set; } = null;
    public SideScrollingCameraPoint CameraPointRight { get; set; } = null;

    // References
    GameplaySystemSetting Setting => GameplaySystem.Setting;
    PlayerAvatarController CurrentPlayer => GameplaySystem.CurrentPlayer;
    Vector3 PlayerFocusPoint => GameplaySystem.CurrentPlayer.transform.position + Setting.playerFocusHeight * Vector3.up;
    bool IsSideScrollingMode => ActiveSideScrollingZoneSet.Count > 0;

    // Fields
    Camera cam;
    float t;

    Vector3 camRelativePositionSetting;
    float camFovSetting;

    Quaternion camRotation;
    Vector3 camRelativePosition;

    void ResetCameraSetting()
    {
        camRelativePositionSetting = Setting.camDefaultRelativePostiion;
        camFovSetting = Setting.camDefaultFov;
    }

    void Awake()
    {
        GameplaySystem.CurrentCamera = this;
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        ResetCameraSetting();
        camRotation = CurrentPlayer.transform.rotation;
        t = 1 - Mathf.Pow(1 - Setting.camInterpolatingRate, Time.fixedDeltaTime);
    }
    void FixedUpdate()
    {
        if (IsSideScrollingMode)
        {
            Vector3 targetRelativePosition = Vector3.zero;
            camRotation = Quaternion.identity;
            float targetFov = 0;
            float weight = (CameraPointLeft == null ? 0 : CameraPointLeft.weight) + (CameraPointRight == null ? 0 : CameraPointRight.weight);
            if (weight > 0)
            {
                if (CameraPointLeft != null)
                {
                    var w = CameraPointLeft.weight / weight;
                    camRotation = CameraPointLeft.transform.rotation;
                    targetRelativePosition += w * CameraPointLeft.RelativePosition;
                    targetFov += w * CameraPointLeft.fov;
                }
                if (CameraPointRight != null)
                {
                    var w = CameraPointRight.weight / weight;
                    camRotation = Quaternion.Lerp(camRotation, CameraPointRight.transform.rotation, w);
                    targetRelativePosition += w * CameraPointRight.RelativePosition;
                    targetFov += w * CameraPointRight.fov;
                }
            }
            // Apply interpolation
            transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, t);
            camRelativePosition = Vector3.Lerp(camRelativePosition, targetRelativePosition, t);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, t);
        }
        transform.position = PlayerFocusPoint + transform.rotation * camRelativePosition;
    }

    void Update()
    {
        if (IsSideScrollingMode) return;

        // Third personal view
        float ix = Input.GetAxis("Mouse X") * InputSystem.Setting.mouseSensitivity.x;
        float iy = Input.GetAxis("Mouse Y") * InputSystem.Setting.mouseSensitivity.y;

        camRotation = Quaternion.AngleAxis(ix, Vector3.up) * camRotation * Quaternion.AngleAxis(iy, Vector3.left);

        // Apply interpolation
        transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, Setting.camInterpolatingRateMouseRotation);
        float t = 1 - Mathf.Pow(1 - Setting.camInterpolatingRate, Time.deltaTime);
        camRelativePosition = Vector3.Lerp(camRelativePosition, camRelativePositionSetting, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFovSetting, t);

        transform.position = PlayerFocusPoint + transform.rotation * camRelativePosition;
    }

    // Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (IsSideScrollingMode)
        {
            float weight = (CameraPointLeft == null ? 0 : CameraPointLeft.weight) + (CameraPointRight == null ? 0 : CameraPointRight.weight);
            if (CameraPointLeft == CameraPointRight) weight *= 0.5f;
            if (weight > 0)
            {
                var pOffset = 0.5f * Setting.playerFocusHeight * Vector3.down;
                var pPlayer = PlayerFocusPoint;
                Gizmos.DrawSphere(pPlayer + Vector3.down * 0.1f, 0.2f);
                if (CameraPointLeft != null)
                {
                    var w = CameraPointLeft.weight / weight;
                    var p = CameraPointLeft.PlayerTargetPosition;
                    Gizmos.DrawLine(pPlayer, p);
                    Gizmos.DrawWireSphere(p + pOffset * w, 0.5f * Setting.playerFocusHeight * w);
                }
                if (CameraPointRight != null)
                {
                    var w = CameraPointRight.weight / weight;
                    var p = CameraPointRight.PlayerTargetPosition;
                    Gizmos.DrawLine(pPlayer, p);
                    Gizmos.DrawWireSphere(p + pOffset * w, 0.5f * Setting.playerFocusHeight * w);
                }
            }
        }
        Gizmos.color = Color.white;
    }


}
