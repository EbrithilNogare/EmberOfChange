using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] StatsManager stats;
    
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<StatsManager>();
        stats.OnHit.AddListener(UpdateHealth);
        stats.OnDeath.AddListener(ShowDeath);
        stats.OnExtinguisherUpdate.AddListener(UpdateExtinguisher);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: UI
    public void UpdateHealth()
    {
        Debug.Log("UI: UpdateHealth");
    }

    //TODO: UI
    public void ShowDeath()
    {
        Debug.Log("UI: ShowDeath");

    }
    
    //TODO: UI
    public void UpdateExtinguisher(int value)
    {
        Debug.Log("UI: UpdateExtinguisher");
    }
}
