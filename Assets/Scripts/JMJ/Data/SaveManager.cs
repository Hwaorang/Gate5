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

    // ����
    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("���� ������ ���� �Ϸ�");
    }

    // �ҷ�����
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            Data = JsonUtility.FromJson<PlayerData>(json);

            Debug.Log("���� ������ �ҷ����� �Ϸ�");
        }
        else
        {
            Data = new PlayerData();

            Save();

            Debug.Log("���ο� ���� ������ ����");
        }
    }

    // ���� ������ ����
    public void ResetData()
    {
        Data = new PlayerData();

        Save();

        Debug.Log("���� ������ �ʱ�ȭ");
    }

    // ��� �߰�
    public void AddGold(int amount)
    {
        Data.gold += amount;

        Save();
    }

    // ��� ���
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