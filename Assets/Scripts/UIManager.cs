using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] StatsManager stats;

    //[SerializeField] TMP_Text healthText;
    //[SerializeField] TMP_Text fireExtinguisherText;
    
    [SerializeField] GameObject FEParent;

    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<StatsManager>();
        stats.OnHit.AddListener(UpdateHealth);
        stats.OnDeath.AddListener(ShowDeath);
        stats.OnExtinguisherUpdate.AddListener(UpdateExtinguisher);
        //healthText.text = "Health: " + stats.health;
        FEParent.transform.GetChild(0).gameObject.SetActive(true);
        //fireExtinguisherText.text = "FE: " + stats.fireExtinguisher;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO: UI
    public void UpdateHealth()
    {
        Debug.Log("UI: UpdateHealth");
        //healthText.text = "Health: " + stats.health;
    }

    //TODO: UI
    public void ShowDeath()
    {
        Debug.Log("UI: ShowDeath");

    }

    //TODO: UI
    public void UpdateExtinguisher(int value)
    {
        Debug.Log("UI: UpdateExtinguisher " + value);
        foreach (Transform child in FEParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        FEParent.transform.GetChild(value).gameObject.SetActive(true);
        //fireExtinguisherText.text = "FE: " + stats.fireExtinguisher;
    }
}
