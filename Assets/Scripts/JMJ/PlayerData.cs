using UnityEngine;
using System;
//저장할 데이터의 형태
//처음 게임을 실행할때 기본
[Serializable]
public class PlayerData
{
    // 골드
    public int gold = 1000;

    // 선택한 스킨 번호
    public int selectedSkin = 0;

    // 난이도
    // 0 = Easy
    // 1 = Normal
    // 2 = Hard
    public int selectedDifficulty = 1;

    // 강화 레벨
    public int attackLevel = 0;
    public int speedLevel = 0;
    public int hpLevel = 0;
}
