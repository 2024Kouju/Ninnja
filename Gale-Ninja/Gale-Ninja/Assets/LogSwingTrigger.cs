using UnityEngine;
using System.Collections;

public class LogSwingRightToLeft : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public Transform player;            // プレイヤーのTransform

    [Header("スイング設定")]
    public float triggerDistance = 5f;  // 感知距離
    public float swingAngle = 45f;      // 初期角度（右上）
    public float swingImpulse = 5f;     // スイングの勢い（AddForceではなくAddTorqueで制御）
    public float resetTime = 10f;       // 再び待機状態に戻るまでの時間（秒）

    [Header("見た目オプション（紐）")]
    public Transform anchor;            // 紐の上端位置
    private LineRenderer line;          // 紐を描く用

    private Rigidbody2D rb;
    private bool hasSwung = false;      // 一度だけスイングする用のフラグ

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // スタート時は物理を止め、右上から待機
        rb.isKinematic = true;
        transform.rotation = Quaternion.Euler(0, 0, swingAngle);

        // 紐の見た目を追加
        if (anchor != null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.black;
            line.endColor = Color.black;
            line.positionCount = 2;
        }
    }

    void Update()
    {
        // 紐の描画更新
        if (line != null && anchor != null)
        {
            line.SetPosition(0, anchor.position);
            line.SetPosition(1, transform.position);
        }

        if (player == null || hasSwung) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // プレイヤーが近づいたらスイング開始
        if (distance < triggerDistance)
        {
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        hasSwung = true;

        // 物理を有効化（この時点で重力がかかる）
        rb.isKinematic = false;

        yield return new WaitForFixedUpdate();

        // AddTorque を使って回転運動としてスイング開始
        rb.AddTorque(swingImpulse, ForceMode2D.Impulse);

        // 一定時間後に停止して初期位置へ戻す
        yield return new WaitForSeconds(resetTime);

        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.rotation = Quaternion.Euler(0, 0, swingAngle);

        hasSwung = false;
    }
}