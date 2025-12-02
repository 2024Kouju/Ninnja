using UnityEngine;
using UnityEngine.UI;

public class NinjutsuIconManager : MonoBehaviour
{
    [System.Serializable]
    public class NinjutsuUI
    {
        public int ninjutsuID;           // 忍術ID 1～5
        public GameObject normalIcon;    // 未購入アイコン
        public Button purchasedButton;   // 購入済みアイコンをボタン化
    }

    public NinjutsuUI[] ninjutsus; // 5個の忍術UI

    private void Start()
    {
        UpdateIcons();
    }

    private void Update()
    {
        // Rキーで保存データをリセット
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var n in ninjutsus)
            {
                PlayerPrefs.DeleteKey("NinjutsuBought_" + n.ninjutsuID);
            }

            PlayerPrefs.Save(); // 念のため保存

            UpdateIcons(); // アイコン表示を更新
            Debug.Log("忍術購入データをリセットしました");
        }
    }

    private void UpdateIcons()
    {
        foreach (var n in ninjutsus)
        {
            bool isBought = PlayerPrefs.GetInt("NinjutsuBought_" + n.ninjutsuID, 0) == 1;

            if (n.normalIcon != null) n.normalIcon.SetActive(!isBought);
            if (n.purchasedButton != null) n.purchasedButton.gameObject.SetActive(isBought);
        }
    }
}