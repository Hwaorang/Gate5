using TMPro;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text difficultyText;

    private int currentDifficulty;

    private void Start()
    {
        currentDifficulty =
            SaveManager.Instance.Data.selectedDifficulty;

        UpdateUI();
    }

    public void SelectEasy()
    {
        currentDifficulty = 0;

        SaveDifficulty();

        UpdateUI();
    }

    public void SelectNormal()
    {
        currentDifficulty = 1;

        SaveDifficulty();

        UpdateUI();
    }

    public void SelectHard()
    {
        currentDifficulty = 2;

        SaveDifficulty();

        UpdateUI();
    }

    private void SaveDifficulty()
    {
        SaveManager.Instance.Data.selectedDifficulty =
            currentDifficulty;

        SaveManager.Instance.Save();
    }

    private void UpdateUI()
    {
        if (difficultyText == null)
        {
            return;
        }

        switch (currentDifficulty)
        {
            case 0:
                difficultyText.text = "EASY";
                break;

            case 1:
                difficultyText.text = "NORMAL";
                break;

            case 2:
                difficultyText.text = "HARD";
                break;
        }
    }

    public int GetDifficulty()
    {
        return currentDifficulty;
    }
}