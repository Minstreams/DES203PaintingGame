using UnityEngine;
using GameSystem;
using GameSystem.Setting;

public class SideScrollingZone : MonoBehaviour
{
    [MinsHeader("References")]
    [Label] public BoxCollider l_Wall;
    [Label] public BoxCollider r_Wall;
    [Label] public Transform cameraPoints;

    [MinsHeader("Parameters")]
    [Label] public float distance;
    [Tooltip("how far can cameras reach out the side scrolling zone")]
    [Label] public float cameraOverflow;

    [MinsHeader("Events")]
    [Label] public SimpleEvent onEnterZone;
    [Label] public SimpleEvent onExitZone;

    GameplaySystemSetting Setting => GameplaySystem.Setting;


    void Start()
    {
        float centerX = distance * 0.5f;
        float centerY = Setting.invisibleWallHeight * 0.5f;
        float centerZ = (Setting.invisibleWallDepth + Setting.sideScrollingPathWidth) * 0.5f;

        BoxCollider triggerZone = GetComponent<BoxCollider>();
        triggerZone.center = new Vector3(centerX, centerY, 0);
        triggerZone.size = new Vector3(distance, Setting.invisibleWallHeight, Setting.sideScrollingPathWidth);

        Vector3 wallSize = new Vector3(distance, Setting.invisibleWallHeight, Setting.invisibleWallDepth);
        l_Wall.center = new Vector3(centerX, centerY, centerZ);
        l_Wall.size = wallSize;
        r_Wall.center = new Vector3(centerX, centerY, -centerZ);
        r_Wall.size = wallSize;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnterZone?.Invoke();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onExitZone?.Invoke();
        }
    }



    // Debug & GUI
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        var yBias = Vector3.up * 0.01f;

        var p = transform.position + transform.right * distance;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawWireSphere(p, 0.3f);

        Gizmos.color = new Color(0, 0.6f, 0, 0.7f);
        Gizmos.DrawLine(transform.position + yBias, p);

        var offsetZ = Setting.sideScrollingPathWidth * 0.5f * transform.forward;
        var wallSizeX = distance * transform.right;
        var wallSizeZ = Setting.invisibleWallDepth * transform.forward;
        Gizmos.color = new Color(0.6f, 0.1f, 0, 0.7f);
        Gizmos.DrawRay(transform.position + yBias + offsetZ, wallSizeZ);
        Gizmos.DrawRay(transform.position + yBias + offsetZ + wallSizeZ, wallSizeX);
        Gizmos.DrawRay(transform.position + yBias + offsetZ, wallSizeX);
        Gizmos.DrawRay(transform.position + yBias + offsetZ + wallSizeX, wallSizeZ);
        Gizmos.DrawRay(transform.position + yBias - offsetZ, -wallSizeZ);
        Gizmos.DrawRay(transform.position + yBias - offsetZ - wallSizeZ, wallSizeX);
        Gizmos.DrawRay(transform.position + yBias - offsetZ, wallSizeX);
        Gizmos.DrawRay(transform.position + yBias - offsetZ + wallSizeX, -wallSizeZ);


        Gizmos.color = Color.white;
    }
}
