using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandomFromList : MonoBehaviour
{
    [System.Serializable]
    public class WeightedPrefab
    {
        public GameObject prefab;
        public float weight;
    }

    public WeightedPrefab[] prefabs;
    public Transform spawnPoint;

    void Start()
    {
        GameObject selectedPrefab = GetRandomPrefab();
        if (selectedPrefab != null && transform.position.y > 4)
        {
            Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        }
    }

    GameObject GetRandomPrefab()
    {
        float totalWeight = 0f;
        foreach (var prefab in prefabs)
        {
            totalWeight += prefab.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var prefab in prefabs)
        {
            cumulativeWeight += prefab.weight;
            if (randomValue < cumulativeWeight)
            {
                return prefab.prefab;
            }
        }

        return null;
    }
}
