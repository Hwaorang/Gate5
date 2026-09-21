using System;

[Serializable]
public class PlayerData
{
    // =========================
    // 기본 골드 
    // =========================

    public int gold = 0;


    // =========================
    // 기본 스킨
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
    // 업그레이드
    // =========================

   
    public int attackLevel = 0;

    public int speedLevel = 0;

    public int attackSpeedLevel = 0;

    public int goldRewardLevel = 0;
}