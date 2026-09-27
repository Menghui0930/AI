using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour {
    [Header("References")]
    public Animator panelAnimator;
    public TextMeshProUGUI dialogueText;

    [Header("Typewriter")]
    public float charTypeInterval = 0.03f;

    [Header("Player Lock")]
    public MovementStageManager movementManager;
    public AimStageManager aimStageManager;
    public WeaponManager weaponManager;
    public CinemachineBrain CameraBrain;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private System.Action onDialogueComplete;
    private bool lockMovementDuringThis;
    private bool dialogueActive;

    void Update() {
        if (!dialogueActive) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            HandleClick();
        }
    }

    public void StartDialogue(string[] lines, bool lockMovement, System.Action onComplete = null) {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentLineIndex = 0;
        lockMovementDuringThis = lockMovement;
        onDialogueComplete = onComplete;
        dialogueActive = true;

        if (lockMovementDuringThis && movementManager != null) {
            movementManager.Freeze = true;
            aimStageManager.enabled = false;
            weaponManager.enabled = false;
            CameraBrain.enabled = false;
        }

        if (panelAnimator != null) panelAnimator.SetTrigger("Open");

        ShowLine(currentLineIndex);
    }

    void HandleClick() {
        if (isTyping) {
            CompleteCurrentLine();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length) {
            EndDialogue();
        } else {
            ShowLine(currentLineIndex);
        }
    }

    void ShowLine(int index) {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLines[index]));
    }

    IEnumerator TypeLine(string line) {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line) {
            dialogueText.text += c;
            yield return new WaitForSeconds(charTypeInterval);
        }

        isTyping = false;
    }

    void CompleteCurrentLine() {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = currentLines[currentLineIndex];
        isTyping = false;
    }

    void EndDialogue() {
        dialogueActive = false;

        if (panelAnimator != null) panelAnimator.SetTrigger("Close");

        if (lockMovementDuringThis && movementManager != null) {
            movementManager.Freeze = false;
            aimStageManager.enabled = true;
            weaponManager.enabled = true;
            CameraBrain.enabled = true;
        }

        onDialogueComplete?.Invoke();
    }
}