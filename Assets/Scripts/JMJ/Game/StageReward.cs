using UnityEngine;

public class StageReward : MonoBehaviour
{
    [Header("스테이지 기본 클리어 골드")]
    [SerializeField] private int baseStageGold = 100;

    private bool rewardGiven = false;


    public void StageClear()
    {
        // 중복 지급 방지
        if (rewardGiven)
        {
            return;
        }

        rewardGiven = true;


        // 저장된 강화 데이터 가져오기
        PlayerData data =
            SaveManager.Instance.Data;


        // 강화 레벨에 따른 추가 골드
        int bonusGold =
            data.goldRewardLevel * 10;


        // 최종 지급 골드
        int totalGold =
            baseStageGold + bonusGold;


        // 골드 지급
        SaveManager.Instance.AddGold(totalGold);


        Debug.Log(
            "스테이지 클리어!" +
            " +" + totalGold + " Gold"
        );
    }
}