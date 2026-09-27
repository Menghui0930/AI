using System.Collections.Generic;
using UnityEngine;

public class RobotGroupController : MonoBehaviour {
    public Transform player;

    [Header("Detect / Lose Range")]
    public float detectRange = 8f;
    public float loseRange = 12f; 

    private List<MeleeEnemyBehaviour> members = new List<MeleeEnemyBehaviour>();
    private bool isAlert = false;

    void Awake() {
        GetComponentsInChildren<MeleeEnemyBehaviour>(true, members);

        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                player = playerObj.transform;
                player = player.transform.parent.GetChild(2);
            }
        }
    }

    void Update() {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!isAlert && dist <= detectRange) {
            isAlert = true;
            NotifyMembers(true);
        } else if (isAlert && dist > loseRange) {
            isAlert = false;
            NotifyMembers(false);
        }
    }

    void NotifyMembers(bool alert) {
        foreach (var m in members) {
            if (m != null) m.SetAlert(alert, player);
        }
    }

    public void RefreshMembers() {
        members.Clear();
        GetComponentsInChildren<MeleeEnemyBehaviour>(true, members);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}