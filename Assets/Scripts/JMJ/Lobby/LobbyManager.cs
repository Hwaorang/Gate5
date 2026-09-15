using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private string gameSceneName = "Game";

    public void StartGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.");
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
        Debug.Log("GoodBye");

        Application.Quit();
    }
}