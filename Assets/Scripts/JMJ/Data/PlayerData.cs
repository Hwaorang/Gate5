using System;

[Serializable]
public class PlayerData
{
    // =========================
    // 재화
    // =========================

    public int gold = 1000;


    // =========================
    // 선택한 스킨
    // =========================

    public int selectedSkin = 0;


    // =========================
    // 난이도
    // =========================

    // 0 = Easy
    // 1 = Normal
    // 2 = Hard

    public int selectedDifficulty = 1;


    // =========================
    // 플레이어 강화
    // =========================

    // 공격력
    public int attackLevel = 0;

    // 이동속도
    public int speedLevel = 0;

    // 공격속도
    public int attackSpeedLevel = 0;

    // 스테이지 클리어 골드 획득량
    public int goldRewardLevel = 0;
}