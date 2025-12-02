using System.Collections;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    public float speed = 5f;
    public float RanSpeed = 0.5f;
    public float jumpPower = 7f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    public int hearts = 1;
    private bool canAttack = true;

    public PlayerController playerController;
    public Animator animator;

    [Header("攻撃設定")]
    public GameObject attackEffectPrefab;
    public Transform effectSpawnPoint;
    public LayerMask enemyLayer;
    [Tooltip("分身の攻撃範囲を調整可能")]
    public float cloneAttackRange = 5.0f; // Inspectorで自由に設定

    public GameObject cloneAttackEffectPrefab;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 半透明に設定
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        // 10秒後に消滅
        StartCoroutine(DestroyAfterTime(10f));
    }

    void Update()
    {
        // 自動移動
        rb.velocity = new Vector2(RanSpeed * speed, rb.velocity.y);

        // ジャンプ
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
        }

        // 攻撃（プレイヤーと同じタイミング）
        if (Input.GetKeyDown(KeyCode.D) && canAttack)
        {
            Debug.Log("CloneAttack");
            StartCoroutine(Attack());
        }

        // アニメーション同期
        SyncAnimation();
    }

    private void SyncAnimation()
    {
        if (animator == null || playerController == null) return;

        Animator playerAnimator = playerController.GetComponent<Animator>();
        if (playerAnimator == null) return;

        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        animator.Play(stateInfo.shortNameHash, 0, stateInfo.normalizedTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hearts--;
            if (hearts <= 0)
            {
                Destroy(gameObject);
            }
        }
    }



    public IEnumerator Attack()
    {
        canAttack = false;
        if (cloneAttackEffectPrefab == null || effectSpawnPoint == null)
        {
            Debug.LogWarning("分身専用攻撃エフェクトPrefabまたはSpawnPointが設定されていません");
            yield break;
        }

        // 分身専用エフェクトを分身の子オブジェクトとして生成
        GameObject effect = Instantiate(cloneAttackEffectPrefab, effectSpawnPoint.position, Quaternion.identity, transform);

        // 半透明に設定（Prefabで設定済みなら不要）
        SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }

        // 分身の位置から攻撃判定
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, cloneAttackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            playerController.audioSource.Play();
            Destroy(enemy.gameObject);
            PlayerController.score += 100;
            playerController.UpdateScoreUI();
            Debug.Log("Hit");
        }

        Invoke("Invoke_CanAttack", 1.0f);

        Destroy(effect, 1.0f);

    }

    public void Invoke_CanAttack()
    {
        canAttack = true;
    }


    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}