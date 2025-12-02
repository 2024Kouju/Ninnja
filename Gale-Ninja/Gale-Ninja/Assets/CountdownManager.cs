using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    public Text countdownText;       // Canvas上のText
    public float countdownTime = 3f; // 3秒カウント
    public GameObject retryButton; // ← Inspectorで割り当てる
    public GameObject GameOverText;
    public GameObject GameClearText;
    public PlayerController player;  // プレイヤー操作スクリプト
    public MonoBehaviour[] gameScriptsToEnable; // 敵生成などのゲームスクリプト

    void Start()
    {
        retryButton.SetActive(false); // 最初は非表示
        GameOverText.SetActive(false);
        GameClearText.SetActive(false);
        // カウント中はプレイヤー操作無効
        if (player != null)
            player.enabled = false;

        // 他のゲームスクリプトも無効化
        foreach (var script in gameScriptsToEnable)
            script.enabled = false;

        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        int count = (int)countdownTime;

        while (count >= 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = ""; // カウント終了で非表示

        // プレイヤー操作を有効化
        if (player != null)
            player.enabled = true;

        // ゲームスクリプトも有効化
        foreach (var script in gameScriptsToEnable)
            script.enabled = true;
    }
}