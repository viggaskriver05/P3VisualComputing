// ...existing code...
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneLocker : MonoBehaviour
{
    [Header("Tags of timeline GameObjects to watch")]
    [SerializeField] private string[] timelineTags = new string[] { "IntroCutscene" };

    [Header("Will find player and freeze ")]
    [SerializeField] private PlayerMove playerMove;

    [Header("will find FootstepSystem and freeze ")]
    [SerializeField] private FootstepSystem footStepSystem;

    private readonly List<PlayableDirector> directors = new List<PlayableDirector>();
    private bool movementLocked = false;

    void Awake()
    {
        if (playerMove == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                playerMove = player.GetComponent<PlayerMove>();
        }

        if (footStepSystem == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                footStepSystem = player.GetComponent<FootstepSystem>();
        }

        RefreshDirectors();
    }

    void Update()
    {
        // if no directors were found, try to refresh (handles directors instantiated later)
        if (directors.Count == 0)
            RefreshDirectors();

        bool anyPlaying = false;
        for (int i = 0; i < directors.Count; i++)
        {
            var d = directors[i];
            if (d == null) continue;
            if (d.state == PlayState.Playing)
            {
                anyPlaying = true;
                break;
            }
        }

        if (anyPlaying != movementLocked)
            SetMovementLocked(anyPlaying);
    }

    private void RefreshDirectors()
    {
        directors.Clear();
        for (int t = 0; t < timelineTags.Length; t++)
        {
            var tag = timelineTags[t];
            if (string.IsNullOrEmpty(tag)) continue;
            var gos = GameObject.FindGameObjectsWithTag(tag);
            for (int i = 0; i < gos.Length; i++)
            {
                var d = gos[i].GetComponent<PlayableDirector>();
                if (d != null && !directors.Contains(d))
                    directors.Add(d);
            }
        }
    }

    private void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        if (playerMove != null)
            playerMove.enabled = !locked;
        else
            Debug.LogWarning("[CutsceneLocker] PlayerMove reference is null.");

        if (footStepSystem != null)
            footStepSystem.enabled = !locked;
        else
            Debug.LogWarning("[CutsceneLocker] FootStepSystem reference is null.");

        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = locked;
    }
}
// ...existing code...