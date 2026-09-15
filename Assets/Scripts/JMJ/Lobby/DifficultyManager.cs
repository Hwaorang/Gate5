using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text difficultyText;

    private int currentDifficulty;

    private void Start()
    {
        // 저장된 난이도 불러오기
        currentDifficulty =
            SaveManager.Instance.Data.selectedDifficulty;

        UpdateUI();
    }

    // =========================
    // 난이도 선택
    // =========================

    // 쉬움
    public void SelectEasy()
    {
        currentDifficulty = 0;

        SaveDifficulty();
        UpdateUI();
    }

    // 보통
    public void SelectNormal()
    {
        currentDifficulty = 1;

        SaveDifficulty();
        UpdateUI();
    }

    // 어려움
    public void SelectHard()
    {
        currentDifficulty = 2;

        SaveDifficulty();
        UpdateUI();
    }

    // =========================
    // 난이도 저장
    // =========================

    private void SaveDifficulty()
    {
        SaveManager.Instance.Data.selectedDifficulty =
            currentDifficulty;

        SaveManager.Instance.Save();
    }

    // =========================
    // 난이도 UI
    // =========================

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

    // =========================
    // 현재 난이도 가져오기
    // =========================

    public int GetDifficulty()
    {
        return currentDifficulty;
    }

    // =========================
    // 게임 시작
    // =========================

    public void StartGame()
    {
        switch (currentDifficulty)
        {
            case 0:
                // 쉬움
                SceneManager.LoadScene("EasyScene");
                break;

            case 1:
                // 보통
                SceneManager.LoadScene("NormalScene");
                break;

            case 2:
                // 어려움
                SceneManager.LoadScene("HardScene");
                break;

            default:
                Debug.LogError("잘못된 난이도입니다.");
                break;
        }
    }
}

