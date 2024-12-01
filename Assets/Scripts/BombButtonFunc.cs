using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombButtonFunc : MonoBehaviour
{
    BuildingController buildingController;
    private RandomEventManager randomEventManager;

    [SerializeField] private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        if (buildingController == null)
            buildingController = FindObjectOfType<BuildingController>();
        toggle = FindObjectOfType<BombsController>().gameObject.GetComponent<Toggle>();
        
        if(randomEventManager == null)
            randomEventManager = FindObjectOfType<RandomEventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BombColumn()
    {
        buildingController.CollapseColumn(transform.GetSiblingIndex() + 1, 2);
        toggle.isOn = false;
        transform.parent.gameObject.SetActive(false);
        randomEventManager.bombDrop = true;
    }
}
