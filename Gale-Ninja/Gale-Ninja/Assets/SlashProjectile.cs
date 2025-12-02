using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 2f;
    private float direction = 1f;

    public void SetDirection(float dir)
    {
        direction = dir;

        Vector3 sc = transform.localScale;
        sc.x = Mathf.Abs(sc.x) * dir;
        transform.localScale = sc;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);   // 時間で消える
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);  // 敵を削除

            // スコア加算
            PlayerController.score += 100;

            // UI更新
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                PlayerController player = playerObj.GetComponent<PlayerController>();
                player.UpdateScoreUI();
            }

            // Destroy(gameObject); ← 斬撃は消さない（貫通）
        }
    }
}
