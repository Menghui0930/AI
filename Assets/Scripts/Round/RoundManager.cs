using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour {
    [Header("Spawn Points（把 Melee / VirusInjector / Sniper 生成点都拖进来）")]
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

                // Round 1、2 结束进入休息时生成宝箱；Round 3 是最后一轮，不会走到这里
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
            // 每帧检查：如果敌人已经全部清空，提前结束这个 round
            if (AreAllPointsCleared()) {
                break;
            }

            timer -= Time.deltaTime;
            UpdateCountdownUI(timer);

            yield return null;
        }

        // 时间到了但还有敌人没打完，UI 显示 0，但敌人不会被清掉，保留到下一round
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