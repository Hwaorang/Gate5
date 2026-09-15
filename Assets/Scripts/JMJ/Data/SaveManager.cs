using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public PlayerData Data { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "playerData.json"
        );

        Load();
    }

    // =========================
    // 저장
    // =========================

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Save");
    }

    // =========================
    // 불러오기
    // =========================

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            Data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("Load");
        }
        else
        {
            Data = new PlayerData();

            Save();

            Debug.Log("Save");
        }
    }

    // =========================
    // 전체 데이터 초기화
    // =========================

    public void ResetData()
    {
        Data = new PlayerData();

        Save();

        Debug.Log("Reset");
    }

    // =========================
    // 골드 추가
    // =========================

    public void AddGold(int amount)
    {
        Data.gold += amount;
        Save();

        Debug.Log($"골드 추가: {Data.gold}");
    }


    // =========================
    // 골드 사용
    // =========================

    public bool SpendGold(int amount)
    {
        if (Data.gold < amount)
        {
            Debug.Log("Gold가 부족합니다.");
            return false;
        }

        Data.gold -= amount;

        Save();

        Debug.Log($"Gold 사용: -{amount} / 현재 Gold: {Data.gold}");

        return true;
    }

    // =========================
    // 테스트용 골드 설정
    // =========================

    public void SetGold(int amount)
    {
        Data.gold = amount;
        Save();

        Debug.Log($"골드 변경: {Data.gold}");
    }

    // =========================
    // 테스트용 골드 초기화
    // =========================

    public void ResetGold()
    {
        Data.gold = 1000;
        Save();

        Debug.Log($"골드 초기화: {Data.gold}");
    }
}
