using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombsController : MonoBehaviour
{
    private Toggle isBombModeActive;
    private BuildingController buildingController;

    [SerializeField] private GameObject BombsButtonParent;

    [SerializeField] private GameObject ButtonPrefab;
    // Start is called before the first frame update
    void Start()
    {
        isBombModeActive = GetComponent<Toggle>();
        if (buildingController == null)
            buildingController = FindObjectOfType<BuildingController>();

        GenerateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateButtons()
    {
        for (int i = 0; i < buildingController.width - 2; i++)
        {
            var button = Instantiate(ButtonPrefab, BombsButtonParent.transform);
        }
    }

    public void ShowingBombButtons()
    {
        BombsButtonParent.SetActive(isBombModeActive.isOn);
    }
}
