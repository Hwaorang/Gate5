using TMPro;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        RefreshGold();
    }

    private void Update()
    {
        RefreshGold();
    }

    public void RefreshGold()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        int gold =
            SaveManager.Instance.Data.gold;

        goldText.text = "?? " + gold.ToString() + " G";
    }
}