using UnityEngine;

public class DifficultySettings : MonoBehaviour
{
    public int Difficulty { get; private set; }

    private void Awake()
    {
        if (SaveManager.Instance == null)
        {
            Difficulty = 1;
            return;
        }

        Difficulty =
            SaveManager.Instance.Data.selectedDifficulty;

        Debug.Log(
            $"현재 난이도 : {Difficulty}"
        );
    }
}