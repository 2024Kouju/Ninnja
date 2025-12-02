using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SelectNinjutsu : MonoBehaviour
{
    [SerializeField] private GameObject[] allNinjutsuIcons; // 0～6

    private List<GameObject> unlockedIcons = new List<GameObject>();
    private int index = 0;
    private const string KEY_SELECTED = "SelectedNinjutsu";

    void Start()
    {
        LoadUnlockedIcons();
        LoadSelectedIndex();
        ShowCurrent();
    }

    private void LoadUnlockedIcons()
    {
        unlockedIcons.Clear();

        for (int i = 0; i < allNinjutsuIcons.Length; i++)
        {
            allNinjutsuIcons[i].SetActive(false);

            // 0番目は常にアンロック
            if (i == 0)
            {
                unlockedIcons.Add(allNinjutsuIcons[0]);
                continue;
            }

            if (PlayerPrefs.GetInt("NinjutsuBought_" + i, 0) == 1)
                unlockedIcons.Add(allNinjutsuIcons[i]);
        }
    }

    private void LoadSelectedIndex()
    {
        int savedID = PlayerPrefs.GetInt(KEY_SELECTED, 0);
        index = 0;

        for (int i = 0; i < unlockedIcons.Count; i++)
        {
            var nj = unlockedIcons[i].GetComponent<Ninjutsu>();
            if (nj != null && nj.ninjutsuID == savedID)
            {
                index = i;
                break;
            }
        }
    }

    private void ShowCurrent()
    {
        foreach (var icon in unlockedIcons)
            icon.SetActive(false);

        if (unlockedIcons.Count > 0)
            unlockedIcons[index].SetActive(true);
    }

    public void Right()
    {
        if (unlockedIcons.Count == 0) return;
        index = (index + 1) % unlockedIcons.Count;
        ShowCurrent();
    }

    public void Left()
    {
        if (unlockedIcons.Count == 0) return;
        index--;
        if (index < 0) index = unlockedIcons.Count - 1;
        ShowCurrent();
    }

    public void Apply()
    {
        if (unlockedIcons.Count == 0) return;
        var nj = unlockedIcons[index].GetComponent<Ninjutsu>();
        int realID = nj != null ? nj.ninjutsuID : 0;
        PlayerPrefs.SetInt(KEY_SELECTED, realID);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Stage1");
    }

    public void ResetNinjutsuData()
    {
        for (int i = 0; i < allNinjutsuIcons.Length; i++)
            PlayerPrefs.DeleteKey("NinjutsuBought_" + i);

        PlayerPrefs.DeleteKey(KEY_SELECTED);
        PlayerPrefs.Save();

        unlockedIcons.Clear();
        foreach (var icon in allNinjutsuIcons)
            icon.SetActive(false);

        index = 0;
        Debug.Log("忍術データをリセットしました");
    }
}