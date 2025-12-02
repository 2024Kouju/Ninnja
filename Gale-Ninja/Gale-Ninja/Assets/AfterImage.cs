using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public GameObject afterImagePrefab;
    public float interval = 0.1f;
    private float timer;
    public bool enableAfterImage = false; // © Žc‘œON/OFFƒtƒ‰ƒO

    void Update()
    {
        if (!enableAfterImage) return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnAfterImage();
            timer = 0f;
        }
    }

    void SpawnAfterImage()
    {
        GameObject obj = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        Destroy(obj, 0.5f);
    }
}
