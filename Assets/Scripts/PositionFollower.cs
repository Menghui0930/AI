using UnityEngine;

public class PositionFollower : MonoBehaviour {
    [Header("Follow Target")]
    public Transform target; 

    [Header("设置")]
    public bool followRotation = false; 
    public Vector3 offset;             

    private void LateUpdate() {
        if (target == null) return;

        transform.position = target.position + offset;

        if (followRotation) {
            transform.rotation = target.rotation;
        }
    }
}
