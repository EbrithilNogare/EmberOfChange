using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PigenShitController : MonoBehaviour
{
    public PlayerController player;
    public BuildingController buildingController;
    
    public GameObject shitPrefab;
    public GameObject shitSplash;
    public Transform shitParent;
    public List<GameObject> shits = new List<GameObject>();
    public List<Tuple<int,int>> shitsInt = new List<Tuple<int,int>>();
    
    Tween tween;
    int LastplayerPos;

    private int roomWidth = 3;


    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        buildingController = FindObjectOfType<BuildingController>();
        transform.position = new Vector3(0, (buildingController.height + 1) * roomWidth, player.transform.position.z);
        LastplayerPos = player.playerPosition.x * 3;
        transform.GetChild(0).DOLocalMoveY(0.4f, 0.5f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        tween = transform.DOMoveX(LastplayerPos + 1, 1f, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(transform.position.x - player.playerPosition.x) >= 1)
        {
            tween.Kill();
            LastplayerPos = player.playerPosition.x * 3;
            if (player.playerPosition.x == 0) LastplayerPos = 3;
            tween = transform.DOMoveX(LastplayerPos, 0.5f, false);
        }
    }

    public void UpdateShits()
    {
        // Debug.Log("UPDATE SHIT");
        // foreach (var shit in shits)
        // {
        //     if (Math.Abs(player.transform.position.y + 1 - transform.position.y) < 0.5f)
        //     {
        //         Hit();
        //         shits.Remove(shit);
        //     }
        //     else
        //     {
        //         if (shit.transform.position.y == 0)
        //         {
        //             return;
        //         }
        //         shit.transform.DOMoveY((shit.transform.position.y - 3), 0.5f, false);
        //     }
        // }
    }

    public void Shit()
    {
        Debug.Log("SHIT");
        var newShit = Instantiate(shitPrefab, shitParent);
        newShit.GetComponent<Shito>().player = this.player;
        shits.Add(newShit);
        
        newShit.transform.position = new Vector3(transform.position.x, (transform.position.y - 3), -1.1f);
    }

    public void Hit()
    {
        Debug.Log("HIT");
        //shitSplash.SetActive(true);
    }
}
