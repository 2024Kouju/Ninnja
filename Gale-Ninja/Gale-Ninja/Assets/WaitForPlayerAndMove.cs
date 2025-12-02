using UnityEngine;
using System.Collections;

public class WaitForPlayerAndMove : MonoBehaviour
{
    public float speed = 2f;
    public float moveTime = 3f;

    public Transform player;
    public float detectDistance = 5f;

    private void Start()
    {
        // プレイヤーを感知するまで待機
        StartCoroutine(WaitForPlayerAndMove1());
    }

    private IEnumerator WaitForPlayerAndMove1()
    {
        // プレイヤー感知待ち
        while (Vector3.Distance(transform.position, player.position) > detectDistance)
        {
            yield return null;
        }

        // 感知したら上下移動開始
        while (true)
        {
            // 上に移動
            float timer = 0f;
            while (timer < moveTime)
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            // 下に移動
            timer = 0f;
            while (timer < moveTime)
            {
                transform.position += Vector3.down * speed * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

  
}
