using DG.Tweening;
using System;
using UnityEngine;

public class PigenDownController : MonoBehaviour
{
    public PlayerController player;

    Tween tween;
    int LastplayerPos;

    private int roomWidth = 3;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        LastplayerPos = player.playerPosition.y * 3;
        tween = transform.DOMoveY(LastplayerPos + 1, 1f, false);
    }

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
