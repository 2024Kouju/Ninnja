using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public float speed = 5f;
    public float RanSpeed = 0.5f;
    public float jumpPower = 7f;
    private bool isGrounded = false;
    private Rigidbody2D rb;

    [Header("ライフ設定")]
    public int maxHearts = 2;
    private int currentHearts;
    public Image[] hearts;

    [Header("アニメーション設定")]
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string attackAnime = "PlayerAttack";
    public string changeAnime = "PlayerChange";
    private string nowAnime = "";
    private string oldAnime = "";
    private bool AttackFlag = false;
    private Animator animator;

    [Header("攻撃設定")]
    public GameObject attackEffectPrefab;
    public Transform effectSpawnPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;
    public LayerMask EnemyLayer;
    public bool canAttack = true;
    public float cooldownTime = 3.0f;
    public Slider attackCooldownSlider;
    public AudioSource audioSource;

    [Header("UI設定")]
    public GameObject retryButton;
    public GameObject NEXTButton;
    public GameObject GameOverText;
    public GameObject GameClearText;

    [Header("スコア設定")]
    public static int score = 0;
    public Text scoreText;

    public static string gameState = "playing";
    private SpriteRenderer spriteRenderer;

    [Header("分身設定")] 
    public GameObject clonePrefab;
    private GameObject cloneInstance;
    private bool cloneUsed = false; 


    private bool Change = false;
    private bool hasUsedDragon = false;  // ★ 1回だけ使用フラグ
    private bool UsedSlowmotion = false;
    private bool UsedTwotiers = false;


    public static bool Doppel = false;
    public static bool Dragon = false;
    public static bool Denkousekka = false;
    public static bool Cherry = false;
    public static bool Slash = false;

    public static bool No = false;
    private bool Slow = false;
    private bool Twotiers = false;
    

    private bool canDoubleJump = false; // 二段ジャンプ可能フラグ
    private bool doubleJumpActive = false; // 効果中フラグ

    private bool slashMode = false;       // 斬撃モードON/OFF
    private float slashTimer = 0f;        // 残り時間
    public float slashDuration = 15f;     // 15秒
    private bool slashUsed = false;       // 使い切りフラグ

    public GameObject slashPrefab;      // Prefab
    public Transform slashSpawnPoint;   // 発射位置

    public GameObject doubleJumpEffectPrefab; // 効果中のエフェクト
    private GameObject activeDoubleJumpEffect; // 実際に呼び出しているエフェクト
    void Start()
    {
        Time.timeScale = 1f;

        // すでにスコアが保存されていれば復元
        if (PlayerPrefs.HasKey("Score")) 
       { 
            score = PlayerPrefs.GetInt("Score"); 

        }
        currentHearts = maxHearts;
        UpdateHeartsUI();
        UpdateScoreUI(); 
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        animator = GetComponent<Animator>();
        nowAnime = moveAnime; 
        oldAnime = moveAnime;
        gameState = "playing";
        currentHearts = maxHearts; 
        if (attackCooldownSlider != null) 
        { 
            attackCooldownSlider.maxValue = cooldownTime; 
            attackCooldownSlider.value = cooldownTime; 
        } 
        if (retryButton != null) 
        { retryButton.SetActive(false);
            GameOverText.SetActive(false); 
            GameClearText.SetActive(false); 
            NEXTButton.SetActive(false);
        } UpdateScoreUI();

        int selected = PlayerPrefs.GetInt("SelectedNinjutsu", 1);

    


        Debug.Log("選択された忍術ID: " + selected);
    }

        void Update()
    {
        if (No)
        {
            // 忍術なし → 何も発動しない
            Debug.Log("忍術なし、何も起こらない");
        }

        if (Doppel && Input.GetKeyDown(KeyCode.S)&&!cloneUsed)
        {
             float direction = transform.localScale.x > 0 ? 1 : -1;
                // プレイヤーの向き
                Vector3 spawnPos = transform.position + new Vector3(1.5f * direction, 0, 0);
                // 距離3
                cloneInstance = Instantiate(clonePrefab, spawnPos, Quaternion.identity);
                CloneController cloneCtrl = cloneInstance.GetComponent<CloneController>();
                cloneCtrl.playerController = this;
                cloneCtrl.animator = cloneInstance.GetComponent<Animator>();
                cloneCtrl.RanSpeed = this.RanSpeed;
                // 自動移動速度を同期
                cloneUsed = true;
                // 一度だけ生成

        }
        

       
            if (Dragon && Input.GetKeyDown(KeyCode.S) &&!Change&& !hasUsedDragon)
            {
            hasUsedDragon = true; // 使用済みにする
            StartCoroutine(ChangeMode());
            
            }

        if (Denkousekka&&Input.GetKeyDown(KeyCode.S) && !Slow &&!UsedSlowmotion)
        {
            UsedSlowmotion = true;
            StartCoroutine(Slowmotion());

        }

        if (Cherry && Input.GetKeyDown(KeyCode.S) && !Twotiers && !UsedTwotiers)
        {
            UsedTwotiers = true;
            StartCoroutine(Doublejump());
        }
        // ==== Sキーで斬撃モードON ====
        if (Slash&& Input.GetKeyDown(KeyCode.S) && !slashUsed)
        {
            slashMode = true;
            slashTimer = slashDuration;
            slashUsed = true;   // 一度使ったら再使用不可
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (gameState != "playing") return;

        rb.velocity = new Vector2(RanSpeed * speed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.D) && canAttack && !Change)
        {
            StartCoroutine(Attack());

            // 斬撃モード中だけ斬撃を飛ばす
            if (slashMode)
            {
                FireSlash();
            }
        }
        if (Change == false)
        {
            // 地上からのジャンプ
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                isGrounded = false;

                if (doubleJumpActive)
                    canDoubleJump = true; // 二段ジャンプを準備
            }
            // 二段ジャンプ
            else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                canDoubleJump = false;  // 二段ジャンプは1回だけ
            }
        }
        // ==== 斬撃モードの時間管理 ====
        if (slashMode)
        {
            slashTimer -= Time.deltaTime;
            if (slashTimer <= 0)
            {
                slashMode = false;   // モード終了
            }
        }

    }

    void FixedUpdate()
    {
        if (gameState != "playing") return;



        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


       

        if (Change) return;

        if (stateInfo.IsName(attackAnime) && stateInfo.normalizedTime >= 1.0f)
        {
            AttackFlag = false;
            nowAnime = isGrounded ? moveAnime : jumpAnime;
        }

        if (!AttackFlag)
        {
            nowAnime = isGrounded ? moveAnime : jumpAnime;
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        nowAnime = attackAnime;
        AttackFlag = true;

        GameObject effect = Instantiate(attackEffectPrefab, effectSpawnPoint.position, Quaternion.identity, transform);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            audioSource.Play();
            Destroy(enemy.gameObject);
            score += 100;
            UpdateScoreUI();
        }

        float timer = 0f;
        while (timer < cooldownTime)
        {
            timer += Time.deltaTime;
            if (attackCooldownSlider != null)
                attackCooldownSlider.value = timer;
            yield return null;
        }

        if (attackCooldownSlider != null)
            attackCooldownSlider.value = cooldownTime;

        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
       
                Damage(1);
        }
        if(collision.gameObject.CompareTag("Die"))
        {
            Damage(2);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
          
           Damage(1);
            
        }
        if(collision.CompareTag("Die"))
        {
            Damage(2);
        }
        else if (collision.gameObject.CompareTag("Goal"))
        {
            Goal();
        }
        else if (collision.gameObject.CompareTag("Dead"))
        {
            GameOver();
        }
    }




    private void Damage(int amount)
    {
        if (Change) return; // ChangeAnime中はダメージ無効
        currentHearts -= amount;
        UpdateHeartsUI();
        if (currentHearts <= 0)
        {
            GameOver();
        }
        
    }

    private void FireSlash()
    {
        float dir = transform.localScale.x > 0 ? 1 : -1;

        GameObject slash = Instantiate(
            slashPrefab,
            slashSpawnPoint.position,
            Quaternion.identity
        );

        slash.GetComponent<SlashProjectile>().SetDirection(dir);
    }

    private IEnumerator ChangeMode()
    {
        Change = true;
        nowAnime = changeAnime;
        animator.Play(nowAnime);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // 元の重力を保存
        float originalGravity = rb.gravityScale;
        bool originalKinematic = rb.isKinematic;
        bool originalColliderTrigger = col.isTrigger;

        // 物理反応を無効化（これで三角タイルに押し出されなくなる）
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;      // ← ここが重要
        col.isTrigger = true;       // 衝突イベントをトリガーに切り替えたいなら true に（任意）

        // アニメを持ち上げるオフセット（transform移動は物理と競合しない）
        Vector3 offset = new Vector3(0, 1.3f, 0);
        transform.position += offset;

        float timer = 0f;
        bool flashing = false;

        while (timer < 10f)
        {
            timer += Time.deltaTime;

            // 残り3秒で点滅開始
            if (timer > 7f && !flashing)
            {
                flashing = true;
                StartCoroutine(FlashRed(sr, 3f));  // FlashRedがある前提
            }

            // 敵に当たったら倒す（OverlapCircleAll の呼び出しを正す）
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                Destroy(enemy.gameObject);
                score += 100;
                UpdateScoreUI();
            }

            yield return null;
        }

        // 物理を元に戻す
        rb.isKinematic = originalKinematic;
        col.isTrigger = originalColliderTrigger;
        rb.gravityScale = originalGravity;

        Change = false;
        nowAnime = isGrounded ? moveAnime : jumpAnime;
        animator.Play(nowAnime);
    }
    private IEnumerator Doublejump()
    {
        doubleJumpActive = true;
        float duration = 15f;
        float timer = 0f;

        // --- エフェクト開始 ---
        if (doubleJumpEffectPrefab != null)
        {
            // プレイヤーの子オブジェクトとして生成（追従する）
            activeDoubleJumpEffect = Instantiate(doubleJumpEffectPrefab, transform);
        }

        // 効果時間ループ
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // --- エフェクト終了 ---
        if (activeDoubleJumpEffect != null)
        {
            Destroy(activeDoubleJumpEffect);
        }

        doubleJumpActive = false;
        canDoubleJump = false;
    }
    private IEnumerator Slowmotion()
    {
        Slow = true;

        // 敵を遅くする
        EnemyController.speed = 1;
        EnemyController.RanSpeed = 0.1f;

        bool flashing = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // プレイヤー残像ON
        FindObjectOfType<AfterImage>().enableAfterImage = true;

        float time = 0f;
        while (time < 20f)
        {
            time += Time.deltaTime;
            // ★ 残り3秒（=2秒経過）で点滅開始
            if (time > 15f && !flashing)
            {
                flashing = true;
                StartCoroutine(FlashRed(sr, 5f));  // 3秒間点滅
                
            }
            
            yield return null;
        }

        // 敵の速度を戻す
        EnemyController.speed = 5;
        EnemyController.RanSpeed = 0.5f;

        // プレイヤー残像OFF
        FindObjectOfType<AfterImage>().enableAfterImage = false;

        Slow = false;
    }


    private void Goal()
    {
        gameState = "clear";
        // スコアを保存
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        Time.timeScale = 0f;
        NEXTButton.SetActive(true);
        GameClearText.SetActive(true);
    }

    private void GameOver()
    {
        gameState = "over";
        // スコアを保存
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        Time.timeScale = 0f;
        retryButton.SetActive(true);
        GameOverText.SetActive(true);
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // currentHearts 以下は表示、それ以外は非表示
            hearts[i].enabled = (i < currentHearts);
        }
    }

    public void retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator FlashRed(SpriteRenderer sr, float duration)
    {
        float timer = 0f;
        bool visible = true;

        Color normal = Color.white;
        Color red = new Color(1f, 0.4f, 0.4f); // 少し薄めの赤

        while (timer < duration)
        {
            timer += 0.1f; // 点滅間隔
            visible = !visible;

            sr.color = visible ? red : normal;

            yield return new WaitForSeconds(0.1f);
        }

        // 点滅が終わったら色を戻す
        sr.color = normal;
    }
}