using UnityEngine;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    public int deadAnimals = 0;
    public int savedAnimals = 0;

    public int difficulty = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetStore();
    }

    public void ResetStore()
    {
        deadAnimals = 0;
        savedAnimals = 0;
    }
}
