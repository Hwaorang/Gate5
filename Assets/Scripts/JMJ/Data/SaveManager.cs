using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // =====================================================
    // 싱글톤
    // =====================================================

    public static SaveManager Instance { get; private set; }


    // =====================================================
    // 플레이어 데이터
    // =====================================================

    public PlayerData Data { get; private set; }


    // =====================================================
    // 저장 경로
    // =====================================================

    private string savePath;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        // 이미 SaveManager가 존재하면
        // 새로 생성된 SaveManager 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 바뀌어도 유지
        DontDestroyOnLoad(gameObject);


        // 저장 파일 위치
        savePath = Path.Combine(
            Application.persistentDataPath,
            "playerData.json"
        );


        // 데이터 불러오기
        Load();
    }


    // =====================================================
    // 저장
    // =====================================================

    public void Save()
    {
        if (Data == null)
        {
            Data = new PlayerData();
        }

        string json = JsonUtility.ToJson(Data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("게임 데이터 저장 완료");
    }


    // =====================================================
    // 불러오기
    // =====================================================

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            Data = JsonUtility.FromJson<PlayerData>(json);

            // 데이터가 정상적으로 만들어지지 않았다면
            if (Data == null)
            {
                Data = new PlayerData();

                Save();

                Debug.Log("저장 데이터가 잘못되어 기본 데이터로 초기화했습니다.");
            }
        }
        else
        {
            // 저장 파일이 없다면 새 데이터 생성
            Data = new PlayerData();

            Save();

            Debug.Log("새로운 게임 데이터를 생성했습니다.");
        }
    }


    // =====================================================
    // 골드 사용
    // =====================================================

    public bool SpendGold(int amount)
    {
        if (Data == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return false;
        }

        if (amount < 0)
        {
            Debug.LogError("골드 사용량은 음수가 될 수 없습니다.");
            return false;
        }

        // 골드 부족
        if (Data.gold < amount)
        {
            Debug.Log("골드가 부족합니다.");
            return false;
        }

        Data.gold -= amount;

        Save();

        Debug.Log("골드 사용 : " + amount);
        Debug.Log("현재 골드 : " + Data.gold);

        return true;
    }


    // =====================================================
    // 골드 추가
    // =====================================================

    public void AddGold(int amount)
    {
        if (Data == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return;
        }

        if (amount < 0)
        {
            Debug.LogError("추가 골드는 음수가 될 수 없습니다.");
            return;
        }

        Data.gold += amount;

        Save();

        Debug.Log("골드 추가 : " + amount);
        Debug.Log("현재 골드 : " + Data.gold);
    }


    // =====================================================
    // 골드 직접 설정
    // 테스트용
    // =====================================================

    public void SetGold(int amount)
    {
        if (Data == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return;
        }

        Data.gold = Mathf.Max(0, amount);

        Save();

        Debug.Log("골드 설정 : " + Data.gold);
    }


    // =====================================================
    // 골드 초기화
    // SaveTest.cs에서 사용
    // =====================================================

    public void ResetGold()
    {
        if (Data == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return;
        }

        Data.gold = 1000;

        Save();

        Debug.Log("골드를 1000으로 초기화했습니다.");
    }


    // =====================================================
    // 선택 스킨 저장
    // =====================================================

    public void SetSelectedSkin(int skinIndex)
    {
        if (Data == null)
        {
            return;
        }

        Data.selectedSkin = skinIndex;

        Save();

        Debug.Log("선택 스킨 저장 : " + skinIndex);
    }


    // =====================================================
    // 난이도 저장
    //
    // 0 = EASY
    // 1 = NORMAL
    // 2 = HARD
    // =====================================================

    public void SetDifficulty(int difficulty)
    {
        if (Data == null)
        {
            return;
        }

        if (difficulty < 0 || difficulty > 2)
        {
            Debug.LogError(
                "잘못된 난이도입니다. 0 = Easy, 1 = Normal, 2 = Hard"
            );

            return;
        }

        Data.selectedDifficulty = difficulty;

        Save();

        Debug.Log("난이도 저장 : " + difficulty);
    }


    // =====================================================
    // 전체 데이터 초기화
    // =====================================================

    public void ResetData()
    {
        // PlayerData 기본값으로 전부 초기화
        Data = new PlayerData();

        Save();


        Debug.Log("====================================");
        Debug.Log("모든 게임 데이터가 초기화되었습니다.");
        Debug.Log("------------------------------------");
        Debug.Log("골드             : " + Data.gold);
        Debug.Log("선택 스킨         : " + Data.selectedSkin);
        Debug.Log("선택 난이도       : " + Data.selectedDifficulty);
        Debug.Log("공격력 레벨       : " + Data.attackLevel);
        Debug.Log("이동속도 레벨     : " + Data.speedLevel);
        Debug.Log("공격속도 레벨     : " + Data.attackSpeedLevel);
        Debug.Log("골드 보상 레벨    : " + Data.goldRewardLevel);
        Debug.Log("====================================");
    }


    // =====================================================
    // 저장 파일 삭제 후 초기화
    // =====================================================

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);

            Debug.Log("저장 파일을 삭제했습니다.");
        }

        Data = new PlayerData();

        Save();

        Debug.Log("새로운 게임 데이터를 생성했습니다.");
    }
}
