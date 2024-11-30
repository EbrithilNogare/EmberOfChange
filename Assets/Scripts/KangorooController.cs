using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KangorooController : MonoBehaviour
{
    [SerializeField] BuildingController buildingController;
    [SerializeField] PlayerController playerController;
    
    // Start is called before the first frame update
    void Start()
    {
        if(buildingController == null)
            buildingController = FindObjectOfType<BuildingController>();
        if(playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        
        playerController.onAnimalFalling.AddListener(MoveToFallingAnimal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToFallingAnimal(int column)
    {
        transform.DOMoveX(column * buildingController.roomWidth, 0.5f);
    }
}
