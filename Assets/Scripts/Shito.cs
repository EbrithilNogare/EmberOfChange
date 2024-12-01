using UnityEngine;
using UnityEngine.Events;

public class Shito : MonoBehaviour
{
    public PlayerController player;
    public UnityEvent onHit;

    void Update()
    {
        transform.position -= new Vector3(0, 1.5f * Time.deltaTime, 0);
        if (Vector3.Distance(transform.position, player.transform.position + new Vector3(0, 0.3f, 0)) < 2.5f)
        {
            onHit.Invoke();
            Destroy(gameObject);
        }

        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }
}
