using System;

[Serializable]
public class PlayerData
{
    // =====================================================
    // 기본 데이터
    // =====================================================

    // 골드
    public int gold = 0;

    // 선택한 스킨
    public int selectedSkin = 0;


    // =====================================================
    // 난이도
    // =====================================================

    // 0 = EASY
    // 1 = NORMAL
    // 2 = HARD
    public int selectedDifficulty = 1;


    // =====================================================
    // 영구 업그레이드
    // =====================================================

    // 공격력
    public int attackLevel = 0;

    // 이동속도
    public int speedLevel = 0;

    // 공격속도
    public int attackSpeedLevel = 0;

    // 스테이지 클리어 골드 보상
    public int goldRewardLevel = 0;
}

