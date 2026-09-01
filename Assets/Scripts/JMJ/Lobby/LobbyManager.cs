using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("게임 씬 이름")]
    [SerializeField] private string gameSceneName = "Game";

    public void StartGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다.");
            return;
        }

        SaveManager.Instance.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void ResetSaveData()
    {
        SaveManager.Instance.ResetData();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

        Application.Quit();
    }
}