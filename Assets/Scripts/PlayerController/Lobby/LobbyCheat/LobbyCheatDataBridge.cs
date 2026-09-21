using UnityEngine;

public static class LobbyCheatDataBridge
{
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
}