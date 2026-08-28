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

    // 저장
    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("게임 데이터 저장 완료");
    }

    // 불러오기
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            Data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("게임 데이터 불러오기 완료");
        }
        else
        {
            Data = new PlayerData();

            Save();

            Debug.Log("새로운 게임 데이터 생성");
        }
    }

    // 저장 데이터 삭제
    public void ResetData()
    {
        Data = new PlayerData();

        Save();

        Debug.Log("저장 데이터 초기화");
    }

    // 골드 추가
    public void AddGold(int amount)
    {
        Data.gold += amount;

        Save();
    }

    // 골드 사용
    public bool SpendGold(int amount)
    {
        if (Data.gold < amount)
        {
            return false;
        }

        Data.gold -= amount;

        Save();

        return true;
    }
}