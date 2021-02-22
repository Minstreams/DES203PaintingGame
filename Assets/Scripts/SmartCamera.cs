using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// My Smart Camera
/// </summary>
[RequireComponent(typeof(Camera))]
public class SmartCamera : MonoBehaviour
{
    public class SmartCameraPoint
    {
        public Vector2 anchor;
        public Vector2 offset;
        public float distance;
        public float fov;

        public SmartCameraPoint(Vector2 anchor, Vector2 offset, float distance, float fov)
        {
            this.anchor = anchor;
            this.offset = offset;
            this.distance = distance;
            this.fov = fov;
        }
    }
    [Label] public Vector2 defaultOffset;
    [Label] public float distanceThreadhold;    // camera point will be ignored if the distance to player exceeds the threadhold

    [Label] public float reactDistance;
    [LabelRange(0, 1)] public float movingRate;

    float defaultFov;
    float defaultDistance;
    Camera cam;
    Vector2 TargetPos { get => PlayerPlatformController.instance == null ? Vector2.zero : (Vector2)PlayerPlatformController.instance.transform.position; }

    static SmartCamera instance;
    public static List<SmartCameraPoint> cameraPoints = new List<SmartCameraPoint>();

    public static void React(Vector2 direction) => instance?._React(direction);
    public static void ReactBack() => instance?._ReactBack();

    void _React(Vector2 direction)
    {
        transform.Translate(((Vector3)direction + Vector3.back) * reactDistance);
    }

    void _ReactBack()
    {
        transform.Translate(Vector3.back * reactDistance);

    }

    void Awake()
    {
        instance = this;
        cameraPoints.Clear();
        cam = GetComponent<Camera>();
        defaultDistance = transform.position.z;
        defaultFov = cam.fieldOfView;
    }

    void FixedUpdate()
    {

        float fullWeight = 0;
        Vector2 target2D = Vector2.zero;
        float targetZ = 0;
        float targetFov = 0;
        for (int i = 0; i < cameraPoints.Count; i++)
        {
            float d = Vector2.Distance(TargetPos, cameraPoints[i].anchor);
            if (d > distanceThreadhold) continue;
            float weight = 1.0f / (d + 0.001f);

            fullWeight += weight;
            target2D += cameraPoints[i].offset * weight;
            targetZ += cameraPoints[i].distance * weight;
            targetFov += cameraPoints[i].fov * weight;
        }

        if (fullWeight == 0)
        {
            target2D = TargetPos - defaultOffset;
            targetZ = defaultDistance;
            targetFov = defaultFov;
        }
        else
        {
            target2D = target2D / fullWeight + TargetPos;
            targetZ /= fullWeight;
            targetFov /= fullWeight;
        }
        Vector3 target3D = new Vector3(target2D.x, target2D.y, targetZ);

        float t = 1.0f - Mathf.Pow(1.0f - movingRate, Time.deltaTime / Time.timeScale);
        transform.position = Vector3.Lerp(transform.position, target3D, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, t);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector3)((Vector2)transform.position + defaultOffset), 0.6f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position, distanceThreadhold);
    }
}
