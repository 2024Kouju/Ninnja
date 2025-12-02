using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        UpdateScoreUI();
    }

    public static int GetScore()
    {
        return PlayerPrefs.GetInt("Score", 0);
    }

    public static void AddScore(int amount)
    {
        int s = PlayerPrefs.GetInt("Score", 0);
        s += amount;
        PlayerPrefs.SetInt("Score", s);
        PlayerPrefs.Save();
    }

    public static bool SpendScore(int amount)
    {
        int s = PlayerPrefs.GetInt("Score", 0);
        if (s < amount) return false; // ‘«‚è‚È‚¢

        s -= amount;
        PlayerPrefs.SetInt("Score", s);
        PlayerPrefs.Save();
        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteKey("Score");
            UpdateScoreUI();
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "ŒoŒ±’l: " + GetScore().ToString();
    }
}
