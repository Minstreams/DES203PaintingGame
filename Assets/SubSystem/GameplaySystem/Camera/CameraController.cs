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
    public bool IsSideScrollingMode => ActiveSideScrollingZoneSet.Count > 0;
    public Camera Cam { get; private set; }
    public CameraOverrideZone OverrideZone { get; set; } = null;

    // References
    GameplaySystemSetting Setting => GameplaySystem.Setting;
    PlayerAvatarController CurrentPlayer => GameplaySystem.CurrentPlayer;
    Vector3 PlayerFocusPoint => GameplaySystem.CurrentPlayer.FocusPoint;

    // Fields
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
        Cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (CurrentPlayer == null)
        {
            enabled = false;
            return;
        }
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
            Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, targetFov, t);
        }
        transform.position = PlayerFocusPoint + transform.rotation * camRelativePosition;
    }
    void Update()
    {
        if (IsSideScrollingMode) return;
        if (GameplaySystem.IsPaused) return;

        // Third personal view
        float ix = Input.GetAxis("Mouse X") * InputSystem.Setting.mouseSensitivity.x;
        float iy = Input.GetAxis("Mouse Y") * InputSystem.Setting.mouseSensitivity.y;

        if (InputSystem.Setting.mouseInvertY) iy = -iy;

        camRotation = Quaternion.AngleAxis(ix, Vector3.up) * camRotation * Quaternion.AngleAxis(iy, Vector3.left);
        var relaPos = camRelativePositionSetting;
        if (OverrideZone != null)
        {
            camRotation = Quaternion.Slerp(camRotation, OverrideZone.Rot, OverrideZone.rotLerpRate);
            relaPos = Vector3.Lerp(relaPos + Quaternion.Inverse(OverrideZone.Rot) * OverrideZone.offset, Quaternion.Inverse(transform.rotation) * (OverrideZone.Pos - PlayerFocusPoint), OverrideZone.posLerpRate);
        }

        // Apply interpolation
        transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, Setting.camInterpolatingRateMouseRotation);
        float t = 1 - Mathf.Pow(1 - Setting.camInterpolatingRate, Time.deltaTime);
        camRelativePosition = Vector3.Lerp(camRelativePosition, relaPos, t);
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, camFovSetting, t);

        transform.position = PlayerFocusPoint + transform.rotation * camRelativePosition;
    }

    public void ReactBack() => ReactBack(Setting.camReactDefaultPower);
    public void ReactBack(float power)
    {
        camRelativePosition.z -= power;
    }
}
