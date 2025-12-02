using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectAutoDestroy : MonoBehaviour
{
    public float destroyTime = 0.5f; // アニメーションの長さに合わせる

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}

