using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour {

    public GameObject player;
    Vector3 prePlayerPos; 
    void Update() 
    { 
        if (player.transform.position != prePlayerPos) 
        {
            transform.position = new Vector3(player.transform.position.x + 5, 0, -10);
            prePlayerPos = player.transform.position; 
        } 
    } 
}

