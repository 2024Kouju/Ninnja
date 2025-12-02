using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("移動設定")]
    public static float speed = 5f;      // 左右の移動スピード
    public static float RanSpeed = 0.5f; // 向き調整
    private bool isGrounded = false; // 地面についているか判定

    [Header("プレイヤー検知設定")]
    public Transform player;        // プレイヤーをInspectorで指定
    public float detectDistance = 8f; // この距離以内で動く

    Animator animator;
    public string enemyAnime = "Enemy";
    private string nowAnime = "";
    private string oldAnime = "";

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        nowAnime = enemyAnime;
        oldAnime = enemyAnime;
    }

    void Update()
    {
        // プレイヤーとの距離を測る
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < detectDistance)
            {
                // プレイヤーが近い → 動く
                rb.velocity = new Vector2(RanSpeed * -speed, rb.velocity.y);
            }
            else
            {
                // プレイヤーが遠い → 停止
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    void FixedUpdate()
    {
        // アニメーション更新
        if (isGrounded)
        {
            nowAnime = enemyAnime;
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
