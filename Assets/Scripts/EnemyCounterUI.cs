using UnityEngine;
using TMPro;

public class EnemyCounterUI : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI robotCountText;
    public TextMeshProUGUI virusCountText;

    private int robotTotal;
    private int virusTotal;

    void Update() {
        int robotCurrent = FindObjectsOfType<MeleeEnemyBehaviour>().Length;
        int virusCurrent = FindObjectsOfType<VirusInjectorBehaviour>().Length;

        if (robotCountText != null) {
            robotCountText.text = "Robot ( " + robotCurrent + " / " + robotTotal + " )";
        }

        if (virusCountText != null) {
            virusCountText.text = "Virus ( " + virusCurrent + " / " + virusTotal + " )";
        }
    }

    // 由 RoundManager 在每次生成完敌人后呼叫，重新记录这一round的总数
    public void RefreshRoundTotals() {
        robotTotal = FindObjectsOfType<MeleeEnemyBehaviour>().Length;
        virusTotal = FindObjectsOfType<VirusInjectorBehaviour>().Length;
    }
}