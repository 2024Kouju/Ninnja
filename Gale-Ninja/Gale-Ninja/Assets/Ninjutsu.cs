using UnityEngine;
using UnityEngine.UI;

public class Ninjutsu : MonoBehaviour
{
    public int ninjutsuID; // 0 = 忍術なし, 1～6 = 忍術
    public int price;
    [TextArea] public string description;

    public GameObject detailPanel;
    public Text descText;
    public Text priceText;

    private static int pendingBuyID;
    private static int pendingPrice;

    [SerializeField] public GameObject[] deletePanel; // 他の忍術アイコン

    private void Start()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    public void OpenDetailPanel()
    {
        pendingBuyID = ninjutsuID;
        pendingPrice = price;

        // 全アイコン非表示
        foreach (var p in deletePanel)
            p.SetActive(false);

        descText.text = description;
        priceText.text = "必要経験値: " + price;

        detailPanel.SetActive(true);
    }

    public void Buy()
    {
        if (ninjutsuID == 0) return; // 忍術なしは購入不要

        if (PlayerPrefs.GetInt("NinjutsuBought_" + pendingBuyID, 0) == 1)
        {
            Debug.Log("すでに購入済み");
            return;
        }

        if (!Score.SpendScore(pendingPrice))
        {
            Debug.Log("スコア不足");
            return;
        }

        PlayerPrefs.SetInt("NinjutsuBought_" + pendingBuyID, 1);
        PlayerPrefs.SetInt("SelectedNinjutsu", pendingBuyID); // 購入後即選択
        PlayerPrefs.Save();

        Debug.Log("忍術 " + pendingBuyID + " を購入しました");
        detailPanel.SetActive(false);
    }

    public void ClosePanel() => detailPanel.SetActive(false);
}
