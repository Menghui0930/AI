using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour {
    [Header("Spawn Points")]
    public List<MonoBehaviour> spawnPoints;

    [Header("Round Durations（依序对应 Round 1, 2, 3）")]
    public float[] roundDurations = { 90f, 105f, 120f };
    public float restDuration = 15f;

    [Header("UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI countdownText;

    private int currentRound = 0;

    [Header("Chest")]
    public ChestSpawnPoint chestSpawnPoint;

    public GameFlowManager gameFlowManager;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;

    private readonly string[] introLines = new string[]
{
    "Finally, you're here!",
    "I'm your assistant, Drone.",
    "All the machines in this area have been infected by the virus.",
    "You need to eliminate them before the time runs out.",
    "Otherwise, the system will destroy you too.",
    "Don't worry. I'll assist you in combat."
};

    private readonly string[] firstRestLines = new string[]
    {
    "A support chest has appeared! Hurry and claim it!"
    };

    void Start() {
        if (dialogueManager != null) {
            dialogueManager.StartDialogue(introLines, lockMovement: true, onComplete: () =>
            {
                StartCoroutine(RunGame());
            });
        } else {
            StartCoroutine(RunGame());
        }
    }

    IEnumerator RunGame() {
        for (int i = 0; i < roundDurations.Length; i++) {
            currentRound = i + 1;

            SpawnRound(currentRound);

            if (roundText != null) roundText.text = "Round " + currentRound;

            yield return StartCoroutine(CountdownRoutine(roundDurations[i]));

            bool isLastRound = (i == roundDurations.Length - 1);

            if (!isLastRound) {
                if (roundText != null) roundText.text = "Next Round";

                if (chestSpawnPoint != null) {
                    chestSpawnPoint.SpawnChest();
                }

                // 只有第一次休息（Round 1 结束后）才播这句提示
                if (i == 0 && dialogueManager != null) {
                    dialogueManager.StartDialogue(firstRestLines, lockMovement: false);
                }

                yield return StartCoroutine(RestRoutine(restDuration));
            } else {
                if (roundText != null) roundText.text = "All Rounds Cleared";
                if (countdownText != null) countdownText.text = "";
            }
        }
    }
    IEnumerator CountdownRoutine(float duration) {
        float timer = duration;

        while (timer > 0f) {
            if (AreAllPointsCleared()) {
                break;
            }

            timer -= Time.deltaTime;
            UpdateCountdownUI(timer);

            yield return null;
        }

        UpdateCountdownUI(0f);
    }

    IEnumerator RestRoutine(float duration) {
        float timer = duration;

        while (timer > 0f) {
            timer -= Time.deltaTime;
            UpdateCountdownUI(timer);
            yield return null;
        }

        UpdateCountdownUI(0f);
    }

    void UpdateCountdownUI(float timeRemaining) {
        if (countdownText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        countdownText.text = minutes + ":" + seconds.ToString("00");
    }

    void SpawnRound(int roundNumber) {
        foreach (var sp in spawnPoints) {
            IRoundSpawnPoint point = sp as IRoundSpawnPoint;
            if (point != null) {
                point.SpawnForRound(roundNumber);
                Debug.Log("Round " + roundNumber);
            }
        }
    }

    bool AreAllPointsCleared() {
        foreach (var sp in spawnPoints) {
            IRoundSpawnPoint point = sp as IRoundSpawnPoint;
            if (point != null && !point.IsCleared()) {
                return false;
            }
        }
        return true;
    }
}