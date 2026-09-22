using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeToDestroy ) Destroy(this.gameObject);
    }

    /*
    private void OnCollisionEnter(Collision collision) {
        Destroy(this.gameObject);
    }
    */
}
