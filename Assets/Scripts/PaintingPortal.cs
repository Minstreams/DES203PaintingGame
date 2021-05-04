using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class PaintingPortal : MonoBehaviour
{
    static int CurrentPortalID { get; set; } = -1;
    Vector3 PlayerPos => GameplaySystem.CurrentPlayer.transform.position;

    [MinsHeader("Painting Protal", SummaryType.Title, -1)]
    [Label] public SimpleEvent onDisable;
    [Label] public SimpleEvent onEnable;
    [Label] public int id;
    [Label] public string parameterName = "_DistortionIntensity";
    [Label] public SimpleEvent onPortal;
    [Label] public Transform inPoint;
    [Label] public Transform outPoint;
    [MinsHeader("VFX")]
    [Label] public Vector2 distanceRange;
    [Label] public Vector2 distortionRange;
    [MinsHeader("Portal")]
    [Label] public SceneCode targetScene;
    [Label] public int targetPortalId;

    [Label] public bool requireJournal;
    [ConditionalShow("requireJournal")] public int requiredJournalIndex;


    ParticleSystemRenderer pRenderer;

    void Awake()
    {
        pRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        SceneSystem.OnPendingLoadScene += OnPendingLoadScene;
        GameplaySystem.onJournalUnlock += OnJournalUnlock;
    }
    void OnJournalUnlock()
    {
        if (!requireJournal) return;
        if (GameplaySystem.journalUnlocked[requiredJournalIndex]) onEnable?.Invoke();
    }
    void Start()
    {
        if (CurrentPortalID == id)
        {
            GameplaySystem.CurrentPlayer.transform.position = outPoint.position;
            GameplaySystem.CurrentCamera.transform.position = GameplaySystem.CurrentPlayer.FocusPoint;
        }
        if (requireJournal && !GameplaySystem.journalUnlocked[requiredJournalIndex])
        {
            onDisable?.Invoke();
        }
    }
    void Update()
    {
        float dis = Vector3.Distance(PlayerPos, transform.position);
        float res = Mathf.Lerp(distortionRange.x, distortionRange.y, (dis - distanceRange.x) / (distanceRange.y - distanceRange.x));
        pRenderer.material.SetFloat(parameterName, res);
    }
    void OnDestroy()
    {
        SceneSystem.OnPendingLoadScene -= OnPendingLoadScene;
        GameplaySystem.onJournalUnlock -= OnJournalUnlock;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPortal?.Invoke();
            GameFlowSystem.SendGameMessage(GameMessage.Portal);
            CurrentPortalID = targetPortalId;
            SceneSystem.LoadScene(targetScene);
            enabled = false;
        }
    }
    void OnPendingLoadScene()
    {
        if (SceneSystem.SceneToLoad == targetScene) SceneSystem.ConfirmLoadScene();
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos.y = Screen.height - pos.y;
        GUI.Label(new Rect(pos, new Vector2(100, 100)), Vector3.Distance(PlayerPos, transform.position).ToString());
    }
#endif
}
