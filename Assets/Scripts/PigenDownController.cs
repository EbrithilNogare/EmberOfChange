using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PigenDownController : MonoBehaviour
{
    public PlayerController player;
    
    Tween tween;
    int LastplayerPos;

    private int roomWidth = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        LastplayerPos = player.playerPosition.y * 3;
        transform.GetChild(0).DOLocalMoveY(0.4f, 0.5f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        tween = transform.DOMoveY(LastplayerPos + 1, 1f, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(transform.position.y - player.playerPosition.y) >= 1)
        {
            tween.Kill();
            LastplayerPos = player.playerPosition.y * 3;
            if (player.playerPosition.y == 0) LastplayerPos = 3;
            tween = transform.DOMoveY(LastplayerPos, 0.5f, false);
        }
    }
}
