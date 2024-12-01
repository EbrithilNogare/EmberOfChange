using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class Shito : MonoBehaviour
{
    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0,1.5f * Time.deltaTime,0);
        if ((transform.position.y - player.transform.position.y - .8 < 0.3f) &&
            (Math.Abs(transform.position.x - player.transform.position.x) < 1f))
        {
            Debug.Log("SHIIIIIIT on the face");
            Destroy(gameObject);
        }

        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }
}
