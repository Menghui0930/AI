using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour {
    [Header("Spawn Points")]
    public List<MonoBehaviour> spawnPoints;

    [Header("Round Durations")]
    public float[] roundDurations = { 90f, 105f, 120f };
    public float restDuration = 15f;

    [Header("UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI countdownText;

    private int currentRound = 0;

    [Header("Chest")]
    public ChestSpawnPoint chestSpawnPoint;

    void Start() {
        StartCoroutine(RunGame());
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