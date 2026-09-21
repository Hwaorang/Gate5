using UnityEngine;

public class SaveTest : MonoBehaviour
{
    [Header("실시간 테스트 골드")]
    [SerializeField] private int testGold = 1000;

    public void ApplyGold()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다.");
            return;
        }

        SaveManager.Instance.SetGold(testGold);

        Debug.Log($"골드를 {testGold}으로 변경했습니다.");
    }

    public void AddGold()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.AddGold(testGold);

        Debug.Log($"{testGold} 골드를 추가했습니다.");
    }

    public void ResetGold()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.ResetGold();

        Debug.Log("골드를 기본값으로 초기화했습니다.");
    }
}