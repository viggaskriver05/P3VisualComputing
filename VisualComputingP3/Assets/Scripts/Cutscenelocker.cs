using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CutsceneLocker : MonoBehaviour
{
    [Header("Disable these while the cutscene plays")]
    public MonoBehaviour[] movementScripts;          // e.g., your PlayerController, CameraLook, etc.
    public CharacterController characterController;  // optional

    #if ENABLE_INPUT_SYSTEM
    [Header("New Input System (optional)")]
    public InputActionAsset inputActions;
    public string gameplayMap = "Player";
    #endif

    public void Lock()
    {
        foreach (var s in movementScripts) if (s) s.enabled = false;
        if (characterController) characterController.enabled = false;

        #if ENABLE_INPUT_SYSTEM
        if (inputActions && !string.IsNullOrEmpty(gameplayMap))
            inputActions.FindActionMap(gameplayMap, false)?.Disable();
        #endif
    }

    public void Unlock()
    {
        foreach (var s in movementScripts) if (s) s.enabled = true;
        if (characterController) characterController.enabled = true;

        #if ENABLE_INPUT_SYSTEM
        if (inputActions && !string.IsNullOrEmpty(gameplayMap))
            inputActions.FindActionMap(gameplayMap, false)?.Enable();
        #endif
    }
}
