using UnityEngine;

/// <summary>
/// 치트 시스템과 기존 Save 시스템 사이의 연결 담당.
///
/// 기존 SaveManager / PlayerData를 직접 수정하지 않고
/// 치트 기능에서 필요한 작업만 외부에서 처리한다.
///
/// 기존 저장 구조가 변경되면
/// 이 클래스만 수정하면 된다.
/// </summary>
public static class LobbyCheatDataBridge
{
    // =========================
    // 골드 추가
    // =========================

    public static bool AddGold(int amount)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning(
                "[Cheat] SaveManager가 없습니다."
            );

            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        SaveManager.Instance.AddGold(
            amount
        );

        return true;
    }


    // =========================
    // 골드 초기화
    // =========================

    public static bool ResetGold()
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
        {
            Debug.LogWarning(
                "[Cheat] 저장 데이터를 찾을 수 없습니다."
            );

            return false;
        }

        SaveManager.Instance.Data.gold = 0;

        SaveManager.Instance.Save();
#if UNITY_EDITOR
        Debug.Log(
            "[Cheat] Gold Reset → 0"
        );
#endif

        return true;
    }


    // =========================
    // 업그레이드 초기화
    // =========================

    public static bool ResetUpgrades()
    {
        if (SaveManager.Instance == null ||
            SaveManager.Instance.Data == null)
        {
            Debug.LogWarning(
                "[Cheat] 저장 데이터를 찾을 수 없습니다."
            );

            return false;
        }

        PlayerData data =
            SaveManager.Instance.Data;

        // 업그레이드 데이터만 초기화
        data.attackLevel = 0;
        data.speedLevel = 0;
        data.attackSpeedLevel = 0;
        data.goldRewardLevel = 0;

        // 기존 Save 기능 사용
        SaveManager.Instance.Save();

        Debug.Log(
            "[Cheat] Upgrade Reset"
        );

        return true;
    }
}