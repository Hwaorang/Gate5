using System;

[Serializable]
public class PlayerData
{
    public int gold = 0;

    public int selectedSkin = 0;

    // 0 = EASY
    // 1 = NORMAL
    // 2 = HARD
    public int selectedDifficulty = 1;

    public int attackLevel = 0;
    public int speedLevel = 0;
    public int attackSpeedLevel = 0;
    public int goldRewardLevel = 0;
}

