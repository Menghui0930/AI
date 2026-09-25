using UnityEngine;

public class Soul : MonoBehaviour
{
    public int amount = 1;
    public float moveSpeed = 8f;
    public float collectDistance = 5.0f;

    private Transform player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        // 自动飞向 Player
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        // 到达玩家
        if (Vector3.Distance(transform.position, player.position) <= collectDistance)
        {
            PlayerSoul playerSoul = player.GetComponent<PlayerSoul>();

            if (playerSoul != null)
            {
                playerSoul.AddSouls(amount);
            }

            Destroy(gameObject);
        }
    }
}
