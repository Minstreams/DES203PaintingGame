using System.Collections;
using System.Collections.Generic;
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
    PlayerAvatarController CurrentPlayer => GameplaySystem.CurrentPlayer;
    CameraController CurrentCamera => GameplaySystem.CurrentCamera;

    readonly SortedList<float, SideScrollingCameraPoint> cameraList = new SortedList<float, SideScrollingCameraPoint>();

    int playerPositionIndex;    // = index of the nearest camera point on the right

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

        var cams = cameraPoints.GetComponentsInChildren<SideScrollingCameraPoint>();
        foreach (var c in cams)
        {
            cameraList.Add(c.transform.localPosition.x + c.Offset.x, c);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onEnterZone?.Invoke();
            CurrentCamera.ActiveSideScrollingZoneSet.Add(this);

            var playerX = Vector3.Dot(transform.right, CurrentPlayer.transform.position - transform.position);
            var camXList = cameraList.Keys;
            var camPList = cameraList.Values;
            // Initialize player position index
            for (playerPositionIndex = 0; playerPositionIndex < camXList.Count && camXList[playerPositionIndex] < playerX; ++playerPositionIndex) ;
            // register nearby camera points
            if (playerPositionIndex > 0) CurrentCamera.CameraPointLeft = camPList[playerPositionIndex - 1];
            if (playerPositionIndex < cameraList.Count) CurrentCamera.CameraPointRight = camPList[playerPositionIndex];

            StartCoroutine(UpdateCameraWeight());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onExitZone?.Invoke();
            CurrentCamera.ActiveSideScrollingZoneSet.Remove(this);

            StopAllCoroutines();

            // clear nearby camera points
            //var camPList = cameraList.Values;
            //if (playerPositionIndex > 0 && CurrentCamera.CameraPointLeft == camPList[playerPositionIndex - 1]) CurrentCamera.CameraPointLeft = null;
            //if (playerPositionIndex < cameraList.Count && CurrentCamera.CameraPointRight == camPList[playerPositionIndex]) CurrentCamera.CameraPointRight = null;
        }
    }

    readonly WaitForFixedUpdate interval = new WaitForFixedUpdate();

    IEnumerator UpdateCameraWeight()
    {
        while (true)
        {
            var playerX = Vector3.Dot(transform.right, CurrentPlayer.transform.position - transform.position);
            var camXList = cameraList.Keys;
            var camPList = cameraList.Values;

            if (playerPositionIndex > 0)
            {
                var camX = camXList[playerPositionIndex - 1];
                var camP = camPList[playerPositionIndex - 1];
                if (camX > playerX)
                {
                    --playerPositionIndex;
                    CurrentCamera.CameraPointRight = camPList[playerPositionIndex];
                    if (playerPositionIndex > 0) CurrentCamera.CameraPointLeft = camPList[playerPositionIndex - 1];
                }
                else
                {
                    camP.weight = GameplaySystem.CalculateCameraPointWeight(playerX - camX);
                }
            }
            if (playerPositionIndex < cameraList.Count)
            {
                var camX = camXList[playerPositionIndex];
                var camP = camPList[playerPositionIndex];
                if (camX < playerX)
                {
                    ++playerPositionIndex;
                    CurrentCamera.CameraPointLeft = camPList[playerPositionIndex - 1];
                    if (playerPositionIndex < cameraList.Count) CurrentCamera.CameraPointRight = camPList[playerPositionIndex];
                }
                else
                {
                    camP.weight = GameplaySystem.CalculateCameraPointWeight(camX - playerX);
                }
            }

            yield return interval;
        }
    }
}
