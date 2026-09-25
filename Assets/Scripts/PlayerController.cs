using UnityEngine;
/*
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private float horizontal;
    private float vertical;

    // 相机
    public Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // 相机前方
        Vector3 forward = cameraTransform.forward;

        // 相机右方
        Vector3 right = cameraTransform.right;

        // 去掉上下角度
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 根据相机计算移动方向
        Vector3 moveDirection =
            forward * vertical +
            right * horizontal;

        // 防止斜向移动更快
        moveDirection.Normalize();

        // 移动
        rb.MovePosition(
            rb.position +
            moveDirection * moveSpeed * Time.fixedDeltaTime
        );

        // 玩家朝向移动方向
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }

}
*/
