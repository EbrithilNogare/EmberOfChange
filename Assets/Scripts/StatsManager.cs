using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    [SerializeField]
    public float health = 100f;
    public int fireExtinguisher = 5;

    [SerializeField] private int[] playerPosition;

    public UnityEvent OnDeath;
    public UnityEvent OnHit;
    public UnityEvent<int> OnExtinguisherUpdate;

    void Awake()
    {
        health = 100f;
        fireExtinguisher = 5;
        playerPosition = new int[2];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDamage(float damage)
    {
        if (health - damage <= 0)
        {
            OnDeath.Invoke();
            return;
        }
        health -= damage;
        OnHit.Invoke();
    }

    public void UseExtinguisher()
    {
        fireExtinguisher -= 1;
        OnExtinguisherUpdate.Invoke(fireExtinguisher);

    }

    public void FindExtinguisher()
    {
        fireExtinguisher += 1;
        OnExtinguisherUpdate.Invoke(fireExtinguisher);
    }
}
