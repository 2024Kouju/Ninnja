using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemyCircle : MonoBehaviour
{
    [Header("移動設定")]
    public float speed = 5f;          // 左方向の移動スピード
    public float rotateSpeed = 300f;  // 回転スピード
    private bool isGrounded = false;  // 地面に接触しているか

    [Header("プレイヤー検知設定")]
    public Transform player;          // プレイヤー
    public float detectDistance = 8f; // この距離以内で転がる

    private Rigidbody2D rb;
    private CircleCollider2D circleCol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCol = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // プレイヤーが一定距離に入ったら転がる
        if (distance < detectDistance && isGrounded)
        {
            RollMove();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // 転がりながら左へ移動
    void RollMove()
    {
        rb.velocity = new Vector2(-speed, rb.velocity.y);
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player に触れても消さない
        // if (collision.gameObject.CompareTag("Player")) {}

        // Ground に触れたら地面判定
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
